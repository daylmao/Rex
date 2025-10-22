using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Rex.Presentation.Api.Middlewares;
using Trivo.Presentation.API.Filters;

namespace Rex.Presentation.Api.ServicesExtension;

public static class ServiceExtensions
{
    public static void UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Rex",
                Description =
                    "Rex is a community-driven platform for self-taught individuals to stay motivated through challenges, share their progress in posts, and engage with others through reactions.",
                Contact = new OpenApiContact
                {
                    Name = "Dayron Bello",
                    Email = "dayronbp06@gmail.com"
                }
            });

            options.EnableAnnotations();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by your JWT token."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }


    public static void AddFilters(this IMvcBuilder builder)
    {
        builder.AddMvcOptions(options => { options.Filters.Add<ResultFilter>(); });
    }

    public static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = false;
        });
    }
}
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
                Description = "",
                Contact = new OpenApiContact
                {
                    Name = "Dayron Bello",
                    Email = "dayronbp06@gmail.com"
                }
            });
            options.EnableAnnotations();
        });
    }

    public static void AddFilters(this IMvcBuilder builder)
    {
        builder.AddMvcOptions(options =>
        {
            options.Filters.Add<ResultFilter>();
        });
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
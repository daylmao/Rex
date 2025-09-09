using Asp.Versioning;

namespace Rex.Presentation.Api.ServicesExtension;

public static class ServiceExtensions
{
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
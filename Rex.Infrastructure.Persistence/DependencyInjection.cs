using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rex.Infrastructure.Persistence.Context;

namespace Rex.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        #region DbContext

        services.AddDbContext<RexContext>(postgres =>
        {
            postgres.UseNpgsql(configuration.GetConnectionString("Rex"), 
                m => m.MigrationsAssembly("Rex.Infrastructure.Persistence"));
        });

        #endregion
    }
}
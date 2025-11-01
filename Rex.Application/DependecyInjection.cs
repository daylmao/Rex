using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rex.Application.Behavior;
using Rex.Application.Interfaces;
using Rex.Application.Services;

namespace Rex.Application;

public static class DependecyInjection
{
    public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddProblemDetails();
        
        #region Services

        services.AddScoped<ICodeService, CodeService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IWarnUserService, WarnUserService>();
        services.AddScoped<IRemoveUserService, RemoveUserService>();
        services.AddScoped<IChallengeExpirationService, ChallengeExpirationService>();
        services.AddScoped<IUserInGroupService, UserInGroupService>();
        
        #endregion

        services.AddHttpContextAccessor();
    }
}
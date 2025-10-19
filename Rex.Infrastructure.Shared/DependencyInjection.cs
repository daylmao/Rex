using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.SignalR;
using Rex.Configurations;
using Rex.Infrastructure.Shared.Services;
using Rex.Infrastructure.Shared.Services.SignalR;

namespace Rex.Infrastructure.Shared;

public static class DependencyInjection
{
    public static void AddSharedLayer(this IServiceCollection services, IConfiguration configuration)
    {
        #region Configurations

        services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfigurations"));
        services.Configure<JWTConfiguration>(configuration.GetSection("JWTConfigurations"));
        services.Configure<CloudinaryConfiguration>(configuration.GetSection("CloudinaryConfigurations"));

        #endregion

        #region JWT

        services.Configure<JWTConfiguration>(configuration.GetSection("JWTConfigurations"));

        var jwtConfig = configuration.GetSection("JWTConfigurations");
        var key = jwtConfig["Key"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidAudience = jwtConfig["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        logger?.LogInformation("JWT validation successful for user: {UserId}",
                            context.Principal?.FindFirst("sub")?.Value);
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        logger?.LogWarning("JWT authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },

                    OnForbidden = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
                        logger?.LogWarning("Access forbidden for path: {Path}", context.HttpContext.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });

        #endregion

        #region Services

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IAppConnectionService, AppConnectionService>();
        services.AddScoped<IChatNotifier, ChatNotifier>();
        services.AddScoped<IReactionNotifier, ReactionNotifier>();
        services.AddScoped<IFriendshipNotifier, FriendshipNotifier>();
        services.AddScoped<ICommentsNotifier, CommentsNotifier>();
        services.AddScoped<IChallengeNotifier, ChallengeNotifier>();
        
        #endregion

        #region SignalR

        services.AddSignalR();

        #endregion
    }
}
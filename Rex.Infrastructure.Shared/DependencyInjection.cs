using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Services;
using Rex.Configurations;
using Rex.Infrastructure.Shared.Services;

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
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JWTConfigurations:Issuer"],
                ValidAudience = configuration["JWTConfigurations:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTConfigurations:Key"] ?? string.Empty))
            };
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new JWTResponseDto(true, "The token has expired"));
                        return context.Response.WriteAsync(result);
                    }

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var generalResult = JsonConvert.SerializeObject(new JWTResponseDto(true, "Invalid token or authentication error"));
                    return context.Response.WriteAsync(generalResult);
                },
                    
                OnChallenge = c =>
                {
                    c.HandleResponse();
                    c.Response.StatusCode = 401;
                    c.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new JWTResponseDto(true, "An unexpected authentication error occurred"));
                    return c.Response.WriteAsync(result);
                },
                OnForbidden = c =>
                {
                    c.Response.StatusCode = 403;
                    c.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new JWTResponseDto(true,
                        "You are not authorized to access this content"));

                    return c.Response.WriteAsync(result);
                },
            };

        });
            
        #endregion
        
        #region Services
            
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        

        #endregion
    }
}

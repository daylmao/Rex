using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using AuthenticationService = Rex.Infrastructure.Shared.Services.AuthenticationService;
using IAuthenticationService = Rex.Application.Interfaces.IAuthenticationService;

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

        #region Hangfire

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection")));
        
        services.AddHangfireServer(options =>
        {
            options.ServerName = $"Rex-{Environment.MachineName}";
            options.WorkerCount = 3;
            options.Queues = new[] { "critical", "default" };
        });
        
        #endregion

        #region JWT

        services.Configure<JWTConfiguration>(configuration.GetSection("JWTConfigurations"));

        var jwtConfig = configuration.GetSection("JWTConfigurations");
        var key = jwtConfig["Key"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = "Cookies";
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
            })
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/api/v1/auth/github-login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); 
            })
            .AddOAuth("GitHub", options =>
            {
                var githubConfig = configuration.GetSection("GitHubAuthentication");

                var clientId = githubConfig["ClientId"];
                var clientSecret = githubConfig["ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new InvalidOperationException(
                        "GitHub OAuth configuration is missing. Please ensure ClientId and ClientSecret are set in GitHubAuthentication section."
                    );
                }

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;

                options.CallbackPath = "/signin-github";

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.Scope.Add("user:email");
                options.Scope.Add("read:user");

                options.ClaimActions.MapJsonKey("urn:github:id", "id");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:name", "name");
                options.ClaimActions.MapJsonKey("urn:github:email", "email");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                options.SignInScheme = "Cookies";

                options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request,
                            HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        context.RunClaimActions(user.RootElement);

                        if (!context.Identity.HasClaim(c => c.Type == "urn:github:email"))
                        {
                            var emailRequest =
                                new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                            emailRequest.Headers.Accept.Add(
                                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                            emailRequest.Headers.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var emailResponse = await context.Backchannel.SendAsync(emailRequest,
                                HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);

                            if (emailResponse.IsSuccessStatusCode)
                            {
                                var emails =
                                    System.Text.Json.JsonDocument.Parse(await emailResponse.Content
                                        .ReadAsStringAsync());
                                var primaryEmail = emails.RootElement.EnumerateArray()
                                    .FirstOrDefault(e => e.GetProperty("primary").GetBoolean());

                                if (primaryEmail.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                                {
                                    var email = primaryEmail.GetProperty("email").GetString();
                                    if (!string.IsNullOrEmpty(email))
                                    {
                                        context.Identity.AddClaim(
                                            new System.Security.Claims.Claim("urn:github:email", email));
                                    }
                                }
                            }
                        }
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
        services.AddScoped<IInactiveUserNotifier, InactiveUserNotifier>();
        services.AddScoped<IGithubAuthService, GitHubAuthService>();
        services.AddScoped<IUserClaimService, UserClaimService>();

        #endregion

        #region SignalR

        services.AddSignalR();

        #endregion
    }

    public static void ConfigureHangfireJobs(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<IWarnUserService>(
            recurringJobId: "warn-inactive-users",
            queue: "default",
            methodCall: service => service.ProcessWarning(CancellationToken.None),
            cronExpression: Cron.Minutely,
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            }
        );

        RecurringJob.AddOrUpdate<IRemoveUserService>(
            recurringJobId: "remove-inactive-users",
            queue: "critical",
            methodCall: service => service.ProcessRemoval(CancellationToken.None),
            cronExpression: Cron.Daily(4),
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            }
        );
    }
}
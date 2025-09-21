using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Infrastructure.Persistence.Repository;
using Rex.Infrastructure.Persistence.Services;

namespace Rex.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        #region Redis
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        
        #endregion
        
        #region DbContext

        services.AddDbContext<RexContext>(postgres =>
        {
            postgres.UseNpgsql(configuration.GetConnectionString("RexBackend"), 
                m => m.MigrationsAssembly("Rex.Infrastructure.Persistence"));
        });

        #endregion
        
        #region Repositories

        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserRoleRepository, UserRoleRepository>();
        services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddTransient<IChallengeRepository, ChallengeRepository>();
        services.AddTransient<IChatRepository, ChatRepository>();
        services.AddTransient<ICodeRepository, CodeRepository>();
        services.AddTransient<ICommentRepository, CommentRepository>();
        services.AddTransient<IEntityFileRepository, EntityFileRepository>();
        services.AddTransient<IFileRepository, FileRepository>();
        services.AddTransient<IFriendShipRepository, FrienshipRepository>();
        services.AddTransient<IGroupRoleRepository, GroupRoleRepository>();
        services.AddTransient<IMessageRepository, MessageRepository>();
        services.AddTransient<INotificationRepository, NotificationRepository>();
        services.AddTransient<IPostRepository, PostRepository>();
        services.AddTransient<IReactionRepository, ReactionRepository>();
        services.AddTransient<IUserChallengeRepository, UserChallengeRepository>();
        services.AddTransient<IUserChatRepository, UserChatRepository>();
        services.AddTransient<IUserGroupRepository, UserGroupRepository>();
        services.AddTransient<IGroupRepository, GroupRepository>();

        #endregion

        #region Services

        services.AddScoped<IUserRoleService, UserRoleService>();
        #endregion
    }
}
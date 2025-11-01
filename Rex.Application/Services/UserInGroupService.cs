using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Services;

public class UserInGroupService(
    ILogger<UserInGroupService> logger,
    IDistributedCache cache,
    IUserGroupRepository userGroupRepository,
    IUserRepository userRepository
) : IUserInGroupService
{
    public async Task<ResultT<string>> GetUserRoleInGroupAsync(Guid userId, Guid groupId,
        CancellationToken cancellationToken)
    {
        var version = await cache.GetVersionAsync("group-members", groupId, cancellationToken);
        var cacheKey = $"group-members:{userId}:{groupId}:version:{version}";

        var userGroup = await cache.GetOrCreateAsync(
            cacheKey, async () =>
            {
                var user = await userRepository.GetByIdAsync(userId, cancellationToken);
                if (user is null)
                {
                    logger.LogWarning("User with ID {UserId} was not found in the system.", userId);
                    return ResultT<string>.Failure(
                        Error.NotFound("404", "The specified user does not exist.")
                    );
                }

                var userGroup = await userGroupRepository.GetMemberAsync(userId, groupId, cancellationToken);
                if (userGroup is null)
                {
                    logger.LogWarning(
                        "User with ID {UserId} is not associated with group ID {GroupId}.",
                        userId,
                        groupId
                    );

                    return ResultT<string>.Failure(
                        Error.NotFound("404", "The user is not a member of this group.")
                    );
                }

                return userGroup.GroupRole.Role;
            },
            logger,
            cancellationToken: cancellationToken
        );

        return ResultT<string>.Success(userGroup.Value);
    }
}

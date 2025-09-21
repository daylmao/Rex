using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserGroupRepository(RexContext context): GenericRepository<UserGroup>(context), IUserGroupRepository
{
    public async Task<int> GetUserGroupCountAsync(Guid userId, RequestStatus status, CancellationToken cancellationToken) =>
        await context.UserGroup
            .AsNoTracking()
            .Where(ug => ug.UserId == userId && ug.Status == status.ToString())
            .CountAsync(cancellationToken);

    public async Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId, RequestStatus? status, 
        CancellationToken cancellationToken)
    {
        return await context.UserGroup
            .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId 
                                                && ug.Status == status.ToString()
                                               , cancellationToken);
    }

    public async Task<bool> IsGroupPrivateAsync(Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.GroupId == groupId && c.Group.Visibility == GroupVisibility.Private.ToString(), cancellationToken);

    public async Task<bool> IsUserBannedAsync(Guid userId, Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(ug => ug.UserId == userId && ug.GroupId == groupId && ug.Status == RequestStatus.Banned.ToString(), cancellationToken);
}
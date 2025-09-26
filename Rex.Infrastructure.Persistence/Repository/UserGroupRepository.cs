using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;
using GroupRole = Rex.Enum.GroupRole;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserGroupRepository(RexContext context) : GenericRepository<UserGroup>(context), IUserGroupRepository
{
    public async Task<int> GetUserGroupCountAsync(Guid userId, RequestStatus status,
        CancellationToken cancellationToken) =>
        await context.Set<UserGroup>()
            .AsNoTracking()
            .Where(ug => ug.UserId == userId && ug.Status == status.ToString())
            .CountAsync(cancellationToken);

    public async Task<bool> IsUserInGroupAsync(Guid userId, Guid groupId, RequestStatus? status,
        CancellationToken cancellationToken)
    {
        return await context.Set<UserGroup>()
            .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId
                                                && ug.Status == status.ToString()
                , cancellationToken);
    }

    public async Task<bool> IsGroupPrivateAsync(Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.GroupId == groupId && c.Group.Visibility == GroupVisibility.Private.ToString(),
            cancellationToken);

    public async Task<bool> IsUserBannedAsync(Guid userId, Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(
            ug => ug.UserId == userId && ug.GroupId == groupId && ug.Status == RequestStatus.Banned.ToString(),
            cancellationToken);

    public async Task<PagedResult<UserGroup>> GetMembersAsync(Guid groupId, GroupRole? roleFilter, string? searchTerm,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = context.Set<UserGroup>()
            .AsNoTracking()
            .Select(c => new UserGroup
            {
                UserId = c.UserId,
                GroupId = c.GroupId,
                Status = c.Status,
                RequestedAt = c.RequestedAt,
                GroupRole = c.GroupRole,
                User = new User
                {
                    Id = c.User.Id,
                    UserName = c.User.UserName,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Role = c.User.Role,
                    ProfilePhoto = c.User.ProfilePhoto
                }
            })
            .Where(c => c.GroupId == groupId &&
                        c.Status == RequestStatus.Accepted.ToString());

        if (roleFilter.HasValue)
            query = query.Where(c => c.GroupRole.Role == roleFilter.Value.ToString());

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c =>
                EF.Functions.Like(c.User.UserName, $"%{searchTerm}%") ||
                EF.Functions.Like(c.User.FirstName, $"%{searchTerm}%") ||
                EF.Functions.Like(c.User.LastName, $"%{searchTerm}%") ||
                EF.Functions.Like(c.User.FirstName + " " + c.User.LastName, $"%{searchTerm}%"));
        }

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.RequestedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserGroup>(items, count, pageNumber, pageSize);
    }

    public async Task<PagedResult<UserGroup>> GetGroupRequestsAsync(Guid groupId, RequestStatus status, string? searchTerm,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = context.Set<UserGroup>()
            .AsNoTracking()
            .Where(ug => ug.GroupId == groupId && ug.Status == status.ToString())
            .Select(c => new UserGroup()
            {
                UserId = c.UserId,
                GroupId = c.GroupId,
                Status = c.Status,
                RequestedAt = c.RequestedAt,
                User = new User
                {
                    Id = c.User.Id,
                    UserName = c.User.UserName,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    ProfilePhoto = c.User.ProfilePhoto,
                }
            });

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(ug =>
                EF.Functions.Like(ug.User.UserName, $"%{searchTerm}%") ||
                EF.Functions.Like(ug.User.FirstName, $"%{searchTerm}%") ||
                EF.Functions.Like(ug.User.LastName, $"%{searchTerm}%") ||
                EF.Functions.Like(ug.User.FirstName + " " + ug.User.LastName, $"%{searchTerm}%"));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(ug => ug.RequestedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserGroup>(items, total, pageNumber, pageSize);
    }

    public async Task<UserGroup> GetGroupRequestAsync(Guid userId, Guid groupId, CancellationToken cancellationToken) =>
        await context.Set<UserGroup>()
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.GroupId == groupId && c.Status == RequestStatus.Pending.ToString())
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> RequestExistsAsync(Guid userId, Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.UserId == userId 
                                 && u.GroupId == groupId
                                 && u.Status == RequestStatus.Pending.ToString(), cancellationToken);
}
using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Group = Rex.Models.Group;
using UserGroup = Rex.Models.UserGroup;

namespace Rex.Infrastructure.Persistence.Repository;

public class GroupRepository(RexContext context) : GenericRepository<Group>(context), IGroupRepository
{
    public async Task<PagedResult<Group>> GetGroupsByUserIdAsync(Guid userId, int page, int size,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Group>()
            .AsNoTracking()
            .Where(c => c.UserGroups.Any(g => g.UserId == userId))
            .CountAsync(cancellationToken);

        var groups = await context.Set<Group>()
            .AsNoTracking()
            .Where(c => c.UserGroups.Any(g => g.UserId == userId))
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(g => new Group
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Visibility = g.Visibility,
                ProfilePhoto = g.ProfilePhoto,
                CoverPhoto = g.CoverPhoto,
                CreatedAt = g.CreatedAt,
                UserGroups = g.UserGroups
                    .Select(ug => new UserGroup
                    {
                        UserId = ug.UserId,
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Group>(groups, total, page, size);
    }

    public async Task<PagedResult<Group>> GetGroupsPaginatedAsync(Guid userId, int page, int size, CancellationToken cancellationToken)
    {
        var total = await context.Set<Group>()
            .AsNoTracking()
            .Where(c => !c.UserGroups.Any(g => g.UserId == userId))
            .CountAsync(cancellationToken);

        var result = await context.Set<Group>()
            .AsNoTracking()
            .Where(c => !c.UserGroups.Any(g => g.UserId == userId))
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(g => new Group
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Visibility = g.Visibility,
                ProfilePhoto = g.ProfilePhoto,
                CoverPhoto = g.CoverPhoto,
                CreatedAt = g.CreatedAt,
                UserGroups = g.UserGroups
                    .Select(ug => new UserGroup
                    {
                        UserId = ug.UserId
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Group>(result, total, page, size);
    }

    public async Task<bool> GroupExistAsync(Guid groupId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Id == groupId, cancellationToken);

    public async Task<int> GetGroupCountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<Group>()
            .AsNoTracking()
            .Where(c => c.UserGroups.Any(g => g.UserId == userId))
            .CountAsync(cancellationToken);
}
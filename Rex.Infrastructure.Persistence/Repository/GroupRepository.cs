using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Group = Rex.Models.Group;

namespace Rex.Infrastructure.Persistence.Repository;

public class GroupRepository(RexContext context): GenericRepository<Group>(context), IGroupRepository
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
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Group>(groups, total, page, size);
    }

    public async Task<PagedResult<Group>> GetGroupsPaginatedAsync(int page, int size, CancellationToken cancellationToken)
    {
        var total = await context.Set<Group>()
            .AsNoTracking()
            .CountAsync(cancellationToken);
        
        var result = await context.Set<Group>()
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
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
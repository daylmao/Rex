using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserGroupRepository(RexContext context): GenericRepository<UserGroup>(context), IUserGroupRepository
{
    public async Task<int> GetUserGroupCountAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<UserGroup>()
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .CountAsync(cancellationToken);
    
}
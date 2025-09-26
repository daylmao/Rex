using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class GroupRoleRepository(RexContext context): GenericRepository<Models.GroupRole>(context), IGroupRoleRepository
{
    public async Task<GroupRole> GetRoleByNameAsync(Enum.GroupRole role, CancellationToken cancellationToken) =>
        await context.Set<GroupRole>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Role == role.ToString(), cancellationToken);
}
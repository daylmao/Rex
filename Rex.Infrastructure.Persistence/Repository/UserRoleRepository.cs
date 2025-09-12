using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserRoleRepository(RexContext context): GenericRepository<UserRole>(context), IUserRoleRepository
{
    public async Task<bool> RoleExistsAsync(Guid roleId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Id == roleId, cancellationToken);
}
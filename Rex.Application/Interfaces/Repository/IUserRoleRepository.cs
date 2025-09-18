using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IUserRoleRepository: IGenericRepository<UserRole>
{
    Task<bool> RoleExistsAsync(Guid roleId, CancellationToken cancellationToken);
}
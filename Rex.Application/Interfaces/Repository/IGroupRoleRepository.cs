using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IGroupRoleRepository: IGenericRepository<GroupRole>
{
    Task<GroupRole> GetRoleByNameAsync(Enum.GroupRole role, CancellationToken cancellationToken);
}
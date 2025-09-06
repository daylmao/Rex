using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IUserGroupRepository: IGenericRepository<UserGroup>
{
    Task<int> GetUserGroupCountAsync(Guid userId, CancellationToken cancellationToken);
}
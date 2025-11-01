using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IUserInGroupService
{
    Task<ResultT<string>> GetUserRoleInGroupAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
}
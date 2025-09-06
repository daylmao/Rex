using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> ConfirmedAccountAsync(Guid id, CancellationToken cancellationToken);
    
    Task<bool> UserNameInUseAsync(string userName, Guid userId, CancellationToken cancellationToken);
    
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    
    Task<bool> EmailInUseAsync(string email, Guid userId, CancellationToken cancellationToken);
    
    Task UpdatePasswordAsync(User user, string newPassword, CancellationToken cancellationToken);
    
    Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken);
    
    Task<bool> UserNameExistAsync(string userName, CancellationToken cancellationToken);
    
    Task<User> GetUserDetailsAsync(Guid id, CancellationToken cancellationToken);
    
    Task<PagedResult<User>> GetUsersByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken);
    
    Task<PagedResult<User>> GetUsersByNameOrLastnameAsync(Guid groupId, string searchTerm, int page,
        int size, CancellationToken cancellationToken);
    
    Task<PagedResult<User>> GetPendingRequestsByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken);
    
    Task<PagedResult<User>> GetAdministrativeMembersByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken);
}
using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<PagedResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, int page, int size,
        CancellationToken cancellationToken);

}
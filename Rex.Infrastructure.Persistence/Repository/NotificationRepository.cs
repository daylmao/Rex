using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class NotificationRepository(RexContext context): GenericRepository<Notification>(context), INotificationRepository
{
    public async Task<PagedResult<Notification>> GetNotificationsByUserIdAsync(Guid userId, int page, int size,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Notification>()
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .CountAsync(cancellationToken);
        
        var notifications = await context.Set<Notification>()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Notification>(notifications, total, page, size);
    }

}
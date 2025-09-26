using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class MessageRepository(RexContext context): GenericRepository<Message>(context), IMessageRepository
{
    public async Task<PagedResult<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int size, CancellationToken cancellationToken)
    {
        var query = context.Set<Message>()
            .Where(c => c.ChatId == chatId);

        var total = await query.CountAsync(cancellationToken);
        
        var messages = await query
            .Where(c => c.ChatId == chatId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Message>(messages, total, page, size);
            
    }
}
using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ChatRepository(RexContext context): GenericRepository<Chat>(context), IChatRepository
{
    public async Task<PagedResult<Chat>> GetChatsWithLastMessageByUserIdAsync(Guid userId, int page, int size,
        CancellationToken cancellationToken)
    {
        var total = await context.Set<Chat>()
            .AsNoTracking()
            .Where(u => u.UserChats.Any(g => g.UserId == userId))
            .CountAsync(cancellationToken);
        
        var chats = await context.Set<Chat>()
            .Where(u => u.UserChats.Any(g => g.UserId == userId))
            .Include(m => m.UserChats)
                .ThenInclude(c => c.Chat)
                    .ThenInclude(m => m.Messages)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Chat>(chats, total, page, size);
    }

    public async Task<bool> ChatExistsAsync(Guid chatId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Id == chatId, cancellationToken);
}
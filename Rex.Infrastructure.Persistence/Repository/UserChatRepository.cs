using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserChatRepository(RexContext context) : GenericRepository<UserChat>(context), IUserChatRepository
{
    public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.UserId == userId && u.ChatId == chatId, cancellationToken);
    public async Task<List<Guid>> GetUserChatsAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<UserChat>()
            .Where(c => c.UserId == userId)
            .Select(c => c.ChatId)
            .ToListAsync(cancellationToken);
    
    public async Task CreateRangeAsync(IEnumerable<UserChat> entities, CancellationToken cancellationToken)
    {
        context.Set<UserChat>().AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<User> GetOtherUserInChatAsync(Guid chatId, Guid currentUserId, CancellationToken cancellationToken)
        => await context.Set<UserChat>()
            .Where(uc => uc.ChatId == chatId && uc.UserId != currentUserId)
            .Select(uc => uc.User)
            .FirstOrDefaultAsync(cancellationToken);

}
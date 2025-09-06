using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserChatRepository(RexContext context): GenericRepository<UserChat>(context), IUserChatRepository
{
    public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.UserId == userId && u.Id == chatId, cancellationToken);
    
}
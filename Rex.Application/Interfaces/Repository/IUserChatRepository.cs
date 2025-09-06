using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IUserChatRepository: IGenericRepository<UserChat>
{
    Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
}
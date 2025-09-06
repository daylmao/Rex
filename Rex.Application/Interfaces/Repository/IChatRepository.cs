using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IChatRepository: IGenericRepository<Chat>
{
    Task<PagedResult<Chat>> GetChatsWithLastMessageByUserIdAsync(Guid userId, int page, int size,
        CancellationToken cancellationToken);
    Task<bool> ChatExistsAsync(Guid chatId, CancellationToken cancellationToken);
}
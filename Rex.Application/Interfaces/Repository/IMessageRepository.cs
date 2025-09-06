using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IMessageRepository: IGenericRepository<Message>
{
    Task<PagedResult<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int size,
        CancellationToken cancellationToken);

}
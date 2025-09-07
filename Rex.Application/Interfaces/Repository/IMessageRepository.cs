using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with messages in the database.
    /// </summary>
    public interface IMessageRepository : IGenericRepository<Message>
    {
        /// <summary>
        /// Gets a paginated list of messages for a specific chat.
        /// </summary>
        /// <param name="chatId">The ID of the chat.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of messages for the chat.</returns>
        Task<PagedResult<Message>> GetMessagesByChatIdAsync(Guid chatId, int page, int size,
            CancellationToken cancellationToken);
    }
}
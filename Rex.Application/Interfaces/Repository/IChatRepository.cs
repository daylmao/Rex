using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with chats in the database.
    /// </summary>
    public interface IChatRepository : IGenericRepository<Chat>
    {
        /// <summary>
        /// Gets a paginated list of chats along with their last message for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of chats with their last message.</returns>
        Task<PagedResult<Chat>> GetChatsWithLastMessageByUserIdAsync(
            Guid userId, int page, int size, string? searchTerm = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Checks if a chat exists by its ID.
        /// </summary>
        /// <param name="chatId">The chat ID.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the chat exists; otherwise false.</returns>
        Task<bool> ChatExistsAsync(Guid chatId, CancellationToken cancellationToken);

        Task<Chat?> GetOneToOneChat(Guid firstUser, Guid secondUser, CancellationToken cancellationToken);
    }
}
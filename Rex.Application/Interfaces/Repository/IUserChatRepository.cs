using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with user-chat relationships in the database.
    /// </summary>
    public interface IUserChatRepository : IGenericRepository<UserChat>
    {
        /// <summary>
        /// Checks if a user is part of a specific chat.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="chatId">The ID of the chat.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the user is in the chat; otherwise false.</returns>
        Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        
        Task<List<Guid>> GetUserChatsAsync(Guid userId, CancellationToken cancellationToken);

        Task CreateRangeAsync(IEnumerable<UserChat> entities, CancellationToken cancellationToken);
        
        Task<User> GetOtherUserInChatAsync(Guid chatId, Guid currentUserId, CancellationToken cancellationToken);
    }
}
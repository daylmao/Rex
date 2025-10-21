using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with users in the database.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Checks if a user's account is confirmed.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the account is confirmed; otherwise false.</returns>
        Task<bool> ConfirmedAccountAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a username is already in use by another user.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the username is in use; otherwise false.</returns>
        Task<bool> UserNameInUseAsync(Guid userId, string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The user with the specified email.</returns>
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if an email is already in use by another user.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the email is in use; otherwise false.</returns>
        Task<bool> EmailInUseAsync(string email, Guid userId, CancellationToken cancellationToken);
        
        Task<bool> EmailInUseByYouAsync(Guid userId, string email,  CancellationToken cancellationToken);

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        /// <param name="user">The user whose password will be updated.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task UpdatePasswordAsync(User user, string newPassword, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if an email exists in the database.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the email exists; otherwise false.</returns>
        Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a username exists in the database.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the username exists; otherwise false.</returns>
        Task<bool> UserNameExistAsync(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves detailed information about a user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The user details.</returns>
        Task<User> GetUserDetailsAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of users in a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of users in the group.</returns>
        Task<PagedResult<User>> GetUsersByGroupIdAsync(Guid groupId, int page, int size,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of users in a group filtered by name or last name.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="searchTerm">The search term for name or last name.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of users matching the search term.</returns>
        Task<PagedResult<User>> GetUsersByNameOrLastnameAsync(Guid groupId, string searchTerm, int page,
            int size, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of users with pending requests in a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of users with pending requests.</returns>
        Task<PagedResult<User>> GetPendingRequestsByGroupIdAsync(Guid groupId, int page, int size,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets a paginated list of administrative members in a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="size">The number of items per page.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A paginated list of administrative members.</returns>
        Task<PagedResult<User>> GetAdministrativeMembersByGroupIdAsync(Guid groupId, int page, int size,
            CancellationToken cancellationToken);

        Task UpdateLastTimeConnectionAsync(Guid userId, bool isActive, CancellationToken cancellationToken);
        
        Task<User> GetUserByCommentIdAsync(Guid parentCommentId, CancellationToken cancellationToken);
        
        Task<User> GetByGitHubIdAsync(string githubId, CancellationToken cancellationToken);

    }
}

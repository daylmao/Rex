using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with refresh tokens in the database.
    /// </summary>
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        /// <summary>
        /// Creates a new refresh token in the database.
        /// </summary>
        /// <param name="token">The refresh token to create.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task CreateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a refresh token by its ID.
        /// </summary>
        /// <param name="tokenId">The ID of the refresh token.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The refresh token with the specified ID.</returns>
        Task<RefreshToken> GetRefreshTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a refresh token is valid.
        /// </summary>
        /// <param name="token">The refresh token to validate.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the token is valid; otherwise false.</returns>
        Task<bool> IsRefreshTokenValid(RefreshToken token, CancellationToken cancellationToken);

        /// <summary>
        /// Marks a refresh token as used.
        /// </summary>
        /// <param name="token">The token value to mark as used.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        Task MarkRefreshTokenAsUsedAsync(string token, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a refresh token has already been used.
        /// </summary>
        /// <param name="tokenId">The ID of the refresh token.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the token has been used; otherwise false.</returns>
        Task<bool> IsRefreshTokenUsedAsync(Guid tokenId, CancellationToken cancellationToken);
    }
}

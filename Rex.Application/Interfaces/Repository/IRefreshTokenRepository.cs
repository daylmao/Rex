using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IRefreshTokenRepository: IGenericRepository<RefreshToken>
{
    Task CreateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken> GetRefreshTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken);
    Task<bool> IsRefreshTokenValid(RefreshToken token, CancellationToken cancellationToken);
    Task MarkRefreshTokenAsUsedAsync(string token, CancellationToken cancellationToken);
    Task<bool> IsRefreshTokenUsedAsync(Guid tokenId, CancellationToken cancellationToken);
}
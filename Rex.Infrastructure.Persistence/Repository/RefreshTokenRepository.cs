using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class RefreshTokenRepository(RexContext context): GenericRepository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task CreateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await context.Set<RefreshToken>().AddAsync(token, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetRefreshTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .FirstOrDefaultAsync( t => t.Id == tokenId, cancellationToken );

    public async Task<bool> IsRefreshTokenValidAsync(Guid userId, string receivedToken, CancellationToken cancellationToken)
    {
        var activeTokens = await GetActiveTokensByUserIdAsync(userId, cancellationToken);

        return activeTokens.Any(t => BCrypt.Net.BCrypt.Verify(receivedToken, t.Value));
    }

    public async Task MarkRefreshTokenAsUsedAsync(string token, CancellationToken cancellationToken)
    {
        var userToken = await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Value == token, cancellationToken);

        if (userToken != null)
        {
            userToken.Used = true;
            await SaveAsync(cancellationToken);
        }
    }

    public async Task RevokeOldRefreshTokensAsync(Guid userId, Guid tokenId,
        CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .Where(c => c.Id != tokenId && !c.Used && c.Expiration > DateTime.UtcNow && c.UserId == userId)
            .ExecuteUpdateAsync(c => c.SetProperty(u => u.Revoked, true), cancellationToken);
            

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .Where(t => t.UserId == userId && !t.Used && !t.Revoked && t.Expiration > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
}
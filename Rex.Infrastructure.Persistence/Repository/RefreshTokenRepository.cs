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
    

    public async Task<bool> IsRefreshTokenValid(RefreshToken token, CancellationToken cancellationToken) =>
        await ValidateAsync(t => t.Value == token.Value && t.Expiration > DateTime.UtcNow && !t.Used, cancellationToken);

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

    public async Task<bool> IsRefreshTokenUsedAsync(Guid tokenId, CancellationToken cancellationToken) =>
        await ValidateAsync(t => t.Id == tokenId && t.Used, cancellationToken);
}
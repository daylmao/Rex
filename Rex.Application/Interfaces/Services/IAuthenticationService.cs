using Rex.Application.DTOs;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Interfaces;

/// <summary>
/// Service responsible for handling authentication tasks.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
    Task<ResultT<TokenResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Configurations;
using Rex.Models;

namespace Rex.Infrastructure.Shared.Services;

public class AuthenticationService(
    ILogger<AuthenticationService> logger,
    IOptions<JWTConfiguration> jwtConfiguration,
    IUserRoleService userRoleService,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IHttpContextAccessor httpContextAccessor
) : IAuthenticationService
{
    private readonly JWTConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", "access"),
        };

        var roles = await userRoleService.GetUserRolesAsync(user.Id, cancellationToken);
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var keyBytes = Encoding.UTF8.GetBytes(_jwtConfiguration.Key!);
        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.DurationInMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        logger.LogInformation("Access token generated for user {UserId}", user.Id);

        return tokenString;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Value = tokenString,
            Expiration = expiration,
            CreatedAt = DateTime.UtcNow
        };

        await refreshTokenRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.RevokeOldRefreshTokensAsync(user.Id, refreshToken.Id, cancellationToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // true in production
            SameSite = SameSiteMode.Strict,
            Expires = expiration
        };

        httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", tokenString, cookieOptions);

        logger.LogInformation("Refresh token generated and cookie set for user {UserId}", user.Id);

        return tokenString;
    }

    public async Task<ResultT<TokenResponseDto>> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return ResultT<TokenResponseDto>.Failure(
                Error.Unauthorized("401", "We couldn't find your refresh token. Try logging in again.")
            );
        }

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _jwtConfiguration.Issuer,
            ValidAudience = _jwtConfiguration.Audience,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key!)),
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken token);

        var jwt = (JwtSecurityToken)token;
        var tokenUserId = Guid.Parse(jwt.Subject);

        if (jwt.Claims.FirstOrDefault(c => c.Type == "type")?.Value != "refresh")
        {
            logger.LogWarning("Provided token is not a refresh token for user {UserId}", tokenUserId);
            return ResultT<TokenResponseDto>.Failure(Error.Unauthorized("401",
                "The provided token is not a refresh token"));
        }

        var tokenValid =
            await refreshTokenRepository.IsRefreshTokenValidAsync(tokenUserId, refreshToken, cancellationToken);

        if (!tokenValid)
        {
            logger.LogWarning("Refresh token is invalid, used, revoked, or expired for user {UserId}", tokenUserId);
            return ResultT<TokenResponseDto>.Failure(Error.Unauthorized("401",
                "Refresh token is invalid, used, revoked, or expired"));
        }

        var userExist = await userRepository.GetByIdAsync(tokenUserId, cancellationToken);

        if (userExist is null)
        {
            logger.LogWarning("User {UserId} not found when refreshing token", tokenUserId);
            return ResultT<TokenResponseDto>.Failure(Error.NotFound("404", "User not found"));
        }

        var newAccessToken = await GenerateTokenAsync(userExist, cancellationToken);
        await GenerateRefreshTokenAsync(userExist, cancellationToken);
        await refreshTokenRepository.MarkRefreshTokenAsUsedAsync(refreshToken, cancellationToken);

        logger.LogInformation("Refresh token successfully used and new access token issued for user {UserId}",
            userExist.Id);

        return ResultT<TokenResponseDto>.Success(new TokenResponseDto(AccessToken: newAccessToken));
    }
}
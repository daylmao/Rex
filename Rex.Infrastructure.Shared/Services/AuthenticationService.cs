using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Configurations;
using Rex.Models;
using UserRole = Rex.Enum.UserRole;

namespace Rex.Infrastructure.Shared.Services;

public class AuthenticationService(
    IOptions<JWTConfiguration> jwtConfiguration,
    IUserRoleService userRoleService,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository
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
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.DurationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
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
        var hashedToken = BCrypt.Net.BCrypt.HashPassword(tokenString);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Value = hashedToken,
            Expiration = expiration,
            CreatedAt = DateTime.UtcNow
        };

        await refreshTokenRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.RevokeOldTokensAsync(user.Id, refreshToken.Id, cancellationToken);

        return tokenString;
    }

    public async Task<ResultT<TokenAnswerDto>> RefreshTokenAsync(User user, string refreshToken, CancellationToken cancellationToken)
    {
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

        if (jwt.Claims.FirstOrDefault(c => c.Type == "type")?.Value != "refresh")
            return ResultT<TokenAnswerDto>.Failure(Error.Unauthorized("401", "The provided token is not a refresh token"));

        var tokenValid = await refreshTokenRepository.IsRefreshTokenValidAsync(user.Id, refreshToken, cancellationToken);

        if (!tokenValid)
            return ResultT<TokenAnswerDto>.Failure(Error.Unauthorized("401", "Refresh token is invalid, used, revoked, or expired"));

        var userId = jwt.Subject;
        
        var userExist = await userRepository.GetByIdAsync(Guid.Parse(userId), cancellationToken);

        if (userExist is null)
            return ResultT<TokenAnswerDto>.Failure(Error.NotFound("404", "User not found"));
            
        var newToken = await GenerateTokenAsync(userExist, cancellationToken);
        var newRefreshToken = await GenerateRefreshTokenAsync(userExist, cancellationToken);

        await refreshTokenRepository.MarkRefreshTokenAsUsedAsync(refreshToken, cancellationToken);

        return ResultT<TokenAnswerDto>.Success(new TokenAnswerDto(
            AccessToken: newToken,
            RefreshToken: newRefreshToken
            ));
    }
}

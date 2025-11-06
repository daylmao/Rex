using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Models;
using UserRole = Rex.Enum.UserRole;

namespace Rex.Infrastructure.Shared.Services;

public class GitHubAuthService(
    IUserRepository userRepository,
    IAuthenticationService authenticationService,
    IUserRoleRepository userRoleRepository,
    IHttpContextAccessor httpContextAccessor
) : IGithubAuthService
{
    public async Task<ResultT<GithubResponseDto>> AuthenticateGitHubUserAsync(
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var githubId = principal.FindFirst("urn:github:id")?.Value;
        var email = principal.FindFirst("urn:github:email")?.Value;
        var username = principal.FindFirst("urn:github:login")?.Value;
        var name = principal.FindFirst("urn:github:name")?.Value;
        var avatarUrl = principal.FindFirst("urn:github:avatar")?.Value;

        if (string.IsNullOrEmpty(githubId) || string.IsNullOrEmpty(username))
        {
            return ResultT<GithubResponseDto>.Failure(
                Error.Failure("400", "Could not retrieve GitHub user information")
            );
        }

        var existingUser = await userRepository.GetByGitHubIdAsync(githubId, cancellationToken);

        if (existingUser != null)
        {
            var existingAccessToken = await authenticationService.GenerateTokenAsync(existingUser, cancellationToken);
            await authenticationService.GenerateRefreshTokenAsync(existingUser, cancellationToken); 

            return ResultT<GithubResponseDto>.Success(new GithubResponseDto(
                AccessToken: existingAccessToken,
                UserId: existingUser.Id
            ));
        }

        if (!string.IsNullOrEmpty(email))
        {
            var userWithEmail = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (userWithEmail != null)
            {
                return ResultT<GithubResponseDto>.Failure(
                    Error.Conflict("409", "An account with this email already exists.")
                );
            }
        }

        var (firstName, lastName) = ParseName(name ?? username);
        var role = await userRoleRepository.GetRoleByNameAsync(UserRole.User.ToString(), cancellationToken);

        var user = new User
        {
            Id = Guid.NewGuid(),
            GitHubId = githubId,
            Email = email ?? $"{username}@github.local",
            UserName = username,
            FirstName = firstName,
            LastName = lastName,
            ProfilePhoto = avatarUrl,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            LastConnection = DateTime.UtcNow,
            ConfirmedAccount = !string.IsNullOrEmpty(email),
            Password = null,
            RoleId = role.Id
        };

        await userRepository.CreateAsync(user, cancellationToken);

        var accessToken = await authenticationService.GenerateTokenAsync(user, cancellationToken);
        await authenticationService.GenerateRefreshTokenAsync(user, cancellationToken); // cookie HttpOnly

        return ResultT<GithubResponseDto>.Success(new GithubResponseDto(
            AccessToken: accessToken,
            UserId: user.Id
        ));
    }

    private static (string FirstName, string LastName) ParseName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (string.Empty, string.Empty);

        var nameParts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        
        return nameParts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (nameParts[0], string.Empty),
            _ => (nameParts[0], nameParts[1])
        };
    }
}
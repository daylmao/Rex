using System.Security.Claims;
using Rex.Application.DTOs.JWT;
using Rex.Application.Utilities;

namespace Rex.Application.Interfaces;

public interface IGithubAuthService
{
    Task<ResultT<GithubResponseDto>> AuthenticateGitHubUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}
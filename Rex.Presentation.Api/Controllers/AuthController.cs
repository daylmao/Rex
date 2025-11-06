using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Users.Commands.Login;
using Rex.Application.Utilities;
using IAuthenticationService = Rex.Application.Interfaces.IAuthenticationService;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(
    IMediator mediator,
    IAuthenticationService authenticationService,
    IGithubAuthService gitHubAuthService
    ): ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Login user",
        Description = "Authenticates a user and returns a token if the credentials are correct."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<TokenResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<TokenResponseDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResultT<TokenResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<TokenResponseDto>))]
    public async Task<ResultT<TokenResponseDto>> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
        
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh access token",
        Description = "Generates a new access token using the refresh token stored in the HTTP-only cookie."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<TokenResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResultT<TokenResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<TokenResponseDto>))]
    public async Task<ResultT<TokenResponseDto>> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        return await authenticationService.RefreshTokenAsync(cancellationToken);
    }

    [HttpGet("github-login")]
    [SwaggerOperation(
        Summary = "Login with GitHub",
        Description = "Redirects the user to the GitHub authentication page for OAuth login."
    )]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login(string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback), new { returnUrl }),
            Items = { { "scheme", "GitHub" } }
        };

        return Challenge(properties, "GitHub");
    }

    [HttpGet("callback")]
    [SwaggerOperation(
        Summary = "GitHub authentication callback",
        Description = "Handles the GitHub OAuth callback, verifies authentication, and redirects the user with the generated tokens."
    )]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Callback(string returnUrl, CancellationToken cancellationToken = default)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("GitHub");

        if (!authenticateResult.Succeeded)
        {
            return BadRequest(new { error = "GitHub authentication failed" });
        }

        var result = await gitHubAuthService.AuthenticateGitHubUserAsync(
            authenticateResult.Principal!,
            cancellationToken
        );

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        var redirectUrl = $"{returnUrl}?accessToken={result.Value!.AccessToken}&userId={result.Value.UserId}";
        return Redirect(redirectUrl);
    }
}
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Code;
using Swashbuckle.AspNetCore.Annotations;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Modules.User.Commands.ConfirmAccount;
using Rex.Application.Modules.User.Commands.Login;
using Rex.Application.Modules.User.Commands.ResendCode;
using Rex.Application.Utilities;
using IAuthenticationService = Rex.Application.Interfaces.IAuthenticationService;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(
    IMediator mediator,
    IAuthenticationService authenticationService,
    IGithubAuthService gitHubAuthService,
    IUserClaims userClaims)
    : ControllerBase
{
    
    [Authorize]
    [HttpPost("confirm-account")]
    [SwaggerOperation(
        Summary = "Confirm account",
        Description = "Confirms a user account using the provided confirmation information."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> ConfirmAccountAsync([FromBody] ConfirmAccountDto code,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new ConfirmAccountCommand(userId, code.Code ), cancellationToken);
    }

    [HttpPost("resend-code")]
    [SwaggerOperation(
        Summary = "Resend confirmation code",
        Description = "Resends the confirmation code to the user in case the previous code expired or was lost."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> ResendCodeAsync([FromBody] ResendCodeCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Login user",
        Description = "Authenticates a user and returns a token if the credentials are correct."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<TokenResponseDto>> LoginAsync([FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh authentication token",
        Description = "Refreshes the JWT token using the provided refresh token."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<TokenResponseDto>> RefreshTokenAsync([FromBody] RefreshTokenDto refreshToken,
        CancellationToken cancellationToken)
    {
        return await authenticationService.RefreshTokenAsync(refreshToken.refreshToken, cancellationToken);
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

        var redirectUrl = $"{returnUrl}?accessToken={result.Value!.AccessToken}&refreshToken={result.Value.RefreshToken}&userId={result.Value.UserId}";
        return Redirect(redirectUrl);
    }
}
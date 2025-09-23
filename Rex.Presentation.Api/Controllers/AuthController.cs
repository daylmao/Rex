using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Interfaces;
using Rex.Application.Modules.User.Commands.ConfirmAccount;
using Rex.Application.Modules.User.Commands.Login;
using Rex.Application.Modules.User.Commands.ResendCode;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(IMediator mediator, IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("confirm-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmAccountAsync([FromBody] ConfirmAccountCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),

        };
    }

    [HttpPost("resend-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResendCodeAsync([FromBody] ResendCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Value);
        return result.Error!.Code switch
        {
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "403" => Forbid(),
            "404" => NotFound(result.Error),
            _ => BadRequest()
        };
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        var result = await authenticationService.RefreshTokenAsync(refreshToken, cancellationToken);
        if (result.IsSuccess)
        {
         return Ok(result.Value);
         
        }
        return result.Error!.Code switch
        {
            "401" => Unauthorized(result.Error),
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error),
        };
        
    }
}
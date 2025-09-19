using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.Modules.User.Commands.ConfirmAccount;
using Rex.Application.Modules.User.Commands.ConfirmEmailChange;
using Rex.Application.Modules.User.Commands.Login;
using Rex.Application.Modules.User.Commands.RegisterUser;
using Rex.Application.Modules.User.Commands.ResendCode;
using Rex.Application.Modules.User.Commands.UpdateEmail;
using Rex.Application.Modules.User.Commands.UpdatePassword;
using Rex.Application.Modules.User.Commands.UpdateUserInformation;
using Rex.Application.Modules.User.Commands.UpdateUsername;
using Rex.Application.Modules.User.Queries.GetUserDetails;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(
    IMediator mediator)
    : ControllerBase
{
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error?.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUserAsync([FromForm] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "409" => Conflict(result.Error),
            "404" => NotFound(result.Error),
            _ => BadRequest()
        };
    }

    [HttpPut("confirm-email-change")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpPut("change-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEmailAsync([FromBody] UpdateEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpPatch("username")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUsernameAsync([FromBody] UpdateUsernameCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            "409" => Conflict(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserInformation([FromForm] UpdateUserInformationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Error.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
    
    [HttpGet("me/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserProfileById([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserDetailsByIdQuery(userId), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error!.Code switch
        {
            "404" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}
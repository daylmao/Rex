using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;
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
using Rex.Application.Modules.Users.Commands.InactiveAccount;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations; // <-- needed

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(
    IMediator mediator)
    : ControllerBase
{
    [HttpPut("password")]
    [SwaggerOperation(
        Summary = "Update user password",
        Description = "Updates the password of an existing user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdatePassword([FromBody] UpdatePasswordCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Registers a new user with the provided information"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<RegisterUserDto>> RegisterUserAsync([FromForm] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("confirm-email-change")]
    [SwaggerOperation(Summary = "Confirm email change",
        Description = "Confirms a pending email change request for the user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> ConfirmEmailAsync([FromBody] ConfirmEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("change-email")]
    [SwaggerOperation(
        Summary = "Update user email",
        Description = "Updates the email of an existing user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> UpdateEmailAsync([FromBody] UpdateEmailCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPatch("username")]
    [SwaggerOperation(
        Summary = "Update username",
        Description = "Updates the username of an existing user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> UpdateUsernameAsync([FromBody] UpdateUsernameCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update user information",
        Description = "Updates general information of an existing user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdateUserInformation([FromForm] UpdateUserInformationCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("me/{userId}")]
    [SwaggerOperation(
        Summary = "Get user profile by ID",
        Description = "Returns profile information of a specific user by ID"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<UserProfileDto>> GetUserProfileById([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetUserDetailsByIdQuery(userId), cancellationToken);
    }

    [HttpGet("{userId}/groups/recommended")]
    [SwaggerOperation(
        Summary = "Get recommended groups for user",
        Description = "Returns paginated list of groups the user is not a member of")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> GetGroupsUserNotIn([FromRoute] Guid userId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetGroupsPaginatedQuery(userId, pageNumber, pageSize), cancellationToken);
    }
    
    [HttpDelete("{userId}/deactivate")]
    [SwaggerOperation(
        Summary = "Deactivate user account",
        Description = "Deactivates the user account with the specified ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ResultT<ResponseDto>> InactivateAccountAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new InactiveAccountCommand(userId), cancellationToken);
    }
}
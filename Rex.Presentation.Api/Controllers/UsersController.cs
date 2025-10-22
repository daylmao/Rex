using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Group;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;
using Rex.Application.Modules.User.Commands.ConfirmEmailChange;
using Rex.Application.Modules.User.Commands.RegisterUser;
using Rex.Application.Modules.User.Commands.UpdateEmail;
using Rex.Application.Modules.User.Commands.UpdatePassword;
using Rex.Application.Modules.User.Commands.UpdateUserInformation;
using Rex.Application.Modules.User.Commands.UpdateUsername;
using Rex.Application.Modules.User.Queries.GetUserDetails;
using Rex.Application.Modules.Users.Commands.ConfirmPasswordChangeByEmail;
using Rex.Application.Modules.Users.Commands.InactiveAccount;
using Rex.Application.Modules.Users.Commands.UpdatePasswordByEmail;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(
    IMediator mediator, IUserClaims userClaims)
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
    public async Task<ResultT<ResponseDto>> UpdatePassword([FromBody] UpdatePasswordDto updatePassword,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new UpdatePasswordCommand(userId, updatePassword.CurrentPassword, updatePassword.NewPassword),
            cancellationToken);
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
    public async Task<ResultT<ResponseDto>> ConfirmEmailAsync([FromBody] ConfirmEmailChangeDto confirmEmailChange,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new ConfirmEmailChangeCommand(userId, confirmEmailChange.Code), cancellationToken);
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
    public async Task<ResultT<ResponseDto>> UpdateEmailAsync([FromBody] UpdateEmailDto updateEmail,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new UpdateEmailCommand(userId, updateEmail.Email, updateEmail.NewEmail),
            cancellationToken);
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
    public async Task<ResultT<ResponseDto>> UpdateUsernameAsync([FromBody] UpdateUsernameDto updateUsername ,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new UpdateUsernameCommand(userId, updateUsername.Username), cancellationToken);
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update user information",
        Description = "Updates general information of an existing user"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> UpdateUserInformation(
        [FromForm] UpdateUserInformationDto updateUserInformation,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new UpdateUserInformationCommand(userId, updateUserInformation.ProfilePhoto,
                updateUserInformation.Firstname, updateUserInformation.Lastname, updateUserInformation.Biography),
            cancellationToken);
    }

    [HttpGet("me")]
    [SwaggerOperation(
        Summary = "Get user profile by ID",
        Description = "Returns profile information of a specific user by ID"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<UserProfileDto>> GetUserProfileById(CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new GetUserDetailsByIdQuery(userId), cancellationToken);
    }

    [HttpGet("groups/recommended")]
    [SwaggerOperation(
        Summary = "Get recommended groups for user",
        Description = "Returns paginated list of groups the user is not a member of")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> GetGroupsUserNotIn(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new GetGroupsPaginatedQuery(userId, pageNumber, pageSize), cancellationToken);
    }
    
    [HttpDelete("deactivate")]
    [SwaggerOperation(
        Summary = "Deactivate user account",
        Description = "Deactivates the user account with the specified ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ResultT<ResponseDto>> InactivateAccountAsync(CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new InactiveAccountCommand(userId), cancellationToken);
    }
    
    [HttpPost("password/reset/request")]
    [SwaggerOperation(
        Summary = "Request password reset",
        Description = "Sends a reset code to the user's email address to allow resetting the password"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> RequestPasswordResetAsync([FromBody] UpdatePasswordByEmailCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("password/reset/confirm")]
    [SwaggerOperation(
        Summary = "Confirm password reset",
        Description = "Confirms the reset code and updates the user's password"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ResultT<ResponseDto>> ConfirmPasswordResetAsync(
        [FromBody] ConfirmPasswordChangeByEmailCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

}
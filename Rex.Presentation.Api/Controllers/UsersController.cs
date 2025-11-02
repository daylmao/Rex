using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Code;
using Rex.Application.DTOs.Group;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Groups.Queries.GetGroupsPaginated;
using Rex.Application.Modules.User.Commands.RegisterUser;
using Rex.Application.Modules.User.Queries.GetUserDetails;
using Rex.Application.Modules.Users.Commands.ConfirmAccount;
using Rex.Application.Modules.Users.Commands.ConfirmEmailChange;
using Rex.Application.Modules.Users.Commands.ConfirmPasswordChangeByEmail;
using Rex.Application.Modules.Users.Commands.InactiveAccount;
using Rex.Application.Modules.Users.Commands.ResendCode;
using Rex.Application.Modules.Users.Commands.UpdateEmail;
using Rex.Application.Modules.Users.Commands.UpdatePassword;
using Rex.Application.Modules.Users.Commands.UpdatePasswordByEmail;
using Rex.Application.Modules.Users.Commands.UpdateUserInformation;
using Rex.Application.Modules.Users.Commands.UpdateUsername;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Rex.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [Authorize]
    [HttpPut("password")]
    [SwaggerOperation(Summary = "Update user password", Description = "Updates the password of an existing user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> UpdatePassword([FromBody] UpdatePasswordDto updatePassword,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new UpdatePasswordCommand(userId, updatePassword.CurrentPassword, updatePassword.NewPassword),
            cancellationToken);
    }

    [Authorize]
    [HttpPost("resend-code")]
    [SwaggerOperation(Summary = "Resend confirmation code", Description = "Resends the confirmation code to the user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> ResendCodeAsync([FromBody] ResendCodeCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user with the provided information")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<RegisterUserDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<RegisterUserDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<RegisterUserDto>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResultT<RegisterUserDto>))]
    public async Task<ResultT<RegisterUserDto>> RegisterUserAsync([FromForm] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [Authorize]
    [HttpPost("confirm-account")]
    [SwaggerOperation(Summary = "Confirm account", Description = "Confirms a user account using the provided code")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> ConfirmAccountAsync([FromBody] ConfirmAccountDto code,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new ConfirmAccountCommand(userId, code.Code), cancellationToken);
    }

    [Authorize]
    [HttpPut("confirm-email-change")]
    [SwaggerOperation(Summary = "Confirm email change", Description = "Confirms a pending email change request")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> ConfirmEmailAsync([FromBody] ConfirmEmailCodeDto code,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new ConfirmEmailChangeCommand(userId, code.Code), cancellationToken);
    }

    [Authorize]
    [HttpPut("change-email")]
    [SwaggerOperation(Summary = "Update user email", Description = "Updates the email of an existing user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> UpdateEmailAsync([FromBody] UpdateEmailDto updateEmail,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new UpdateEmailCommand(userId, updateEmail.Email, updateEmail.NewEmail),
            cancellationToken);
    }

    [Authorize]
    [HttpPatch("username")]
    [SwaggerOperation(Summary = "Update username", Description = "Updates the username of an existing user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> UpdateUsernameAsync([FromBody] UpdateUsernameDto updateUsername,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new UpdateUsernameCommand(userId, updateUsername.Username), cancellationToken);
    }

    [Authorize]
    [HttpPut]
    [SwaggerOperation(Summary = "Update user information", Description = "Updates general information of an existing user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> UpdateUserInformation([FromForm] UpdateUserInformationDto updateUserInformation,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new UpdateUserInformationCommand(userId, updateUserInformation.ProfilePhoto,
                updateUserInformation.Firstname, updateUserInformation.Lastname, updateUserInformation.Biography),
            cancellationToken);
    }

    [Authorize]
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Get user profile by ID", Description = "Returns profile information of the current user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<UserProfileDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<UserProfileDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<UserProfileDto>))]
    public async Task<ResultT<UserProfileDto>> GetUserProfileById(CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetUserDetailsByIdQuery(userId), cancellationToken);
    }

    [Authorize]
    [HttpGet("groups/recommended")]
    [SwaggerOperation(Summary = "Get recommended groups for user", Description = "Returns paginated list of groups the user is not a member of")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<GroupDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<GroupDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<PagedResult<GroupDetailsDto>>))]
    public async Task<ResultT<PagedResult<GroupDetailsDto>>> GetGroupsUserNotIn([FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetGroupsPaginatedQuery(userId, pageNumber, pageSize), cancellationToken);
    }

    [HttpDelete("deactivate")]
    [SwaggerOperation(Summary = "Deactivate user account", Description = "Deactivates the user account")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> InactivateAccountAsync(CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new InactiveAccountCommand(userId), cancellationToken);
    }

    [HttpPost("password/reset/request")]
    [SwaggerOperation(Summary = "Request password reset", Description = "Sends a reset code to the user's email")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> RequestPasswordResetAsync([FromBody] UpdatePasswordByEmailCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("password/reset/confirm")]
    [SwaggerOperation(Summary = "Confirm password reset", Description = "Confirms the reset code and updates the password")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> ConfirmPasswordResetAsync([FromBody] ConfirmPasswordChangeByEmailCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }
}
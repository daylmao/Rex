using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.ConfirmPasswordChangeByEmail;

public class ConfirmPasswordChangeByEmailCommandHandler(
    ILogger<ConfirmPasswordChangeByEmailCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
) : ICommandHandler<ConfirmPasswordChangeByEmailCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ConfirmPasswordChangeByEmailCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("ConfirmPasswordChangeByEmailCommand received as null");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "Something went wrong with your request. Please try again."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"Password reset confirmation requested for non-existing user ID: {request.UserId}");
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "We couldn’t find an account linked to this request."));
        }
        
        var codeValidation = await codeService.ValidateCodeAsync(request.Code, cancellationToken);
        if (!codeValidation.IsSuccess)
        {
            logger.LogWarning($"Invalid code provided for user ID {request.UserId}: {codeValidation.Error}");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("422", "The code you entered is incorrect or has expired. Please request a new one and try again."));
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await userRepository.UpdatePasswordAsync(user, hashedPassword, cancellationToken);

        logger.LogInformation($"Password updated successfully for user ID {request.UserId}");
        return ResultT<ResponseDto>.Success(new("Your password has been updated successfully. You can now sign in with your new password."));
    }
}
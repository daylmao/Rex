using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(
    ILogger<UpdatePasswordCommandHandler> logger,
    IUserRepository userRepository
): ICommandHandler<UpdatePasswordCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to update password.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! We didn't get the information needed to update your password."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for password update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your account."));
        }

        var passwordCheck = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
        if (!passwordCheck)
        {
            logger.LogInformation("Current password is incorrect.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! The current password you entered is incorrect."));
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await userRepository.UpdatePasswordAsync(user, hashedPassword, cancellationToken);

        logger.LogInformation("Password updated successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Great! Your password has been updated successfully."));
    }

}
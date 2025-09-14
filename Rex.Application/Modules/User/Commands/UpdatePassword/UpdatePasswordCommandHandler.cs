using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(
    ILogger<UpdatePasswordCommandHandler> logger,
    IUserRepository userRepository
): ICommandHandler<UpdatePasswordCommand, AnswerDto>
{
    public async Task<ResultT<AnswerDto>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdatePasswordCommand request was null.");
            return ResultT<AnswerDto>.Failure(Error.Failure("400", "Invalid request."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            return ResultT<AnswerDto>.Failure(Error.NotFound("404", "User not found."));
        }
        
        var passwordCheck = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
        if (!passwordCheck)
        {
            logger.LogInformation("Password verification failed for user {UserId}.", request.UserId);
            return ResultT<AnswerDto>.Failure(Error.Failure("400", "Current password is incorrect."));
        }
        
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await userRepository.UpdatePasswordAsync(user, request.NewPassword, cancellationToken);

        logger.LogInformation("Password updated successfully for user {UserId}.", request.UserId);
        
        return ResultT<AnswerDto>.Success(new AnswerDto("Password updated successfully."));
    }
}
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler(
    ILogger<ConfirmAccountCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
): ICommandHandler<ConfirmAccountCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogInformation("ConfirmAccountCommand received is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request cannot be null"));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogInformation("User with ID {UserId} not found", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "User not found"));
        }
        
        var confirmUser = await codeService.ConfirmCodeAsync(user.Id, request.Code, cancellationToken);
        if (!confirmUser.IsSuccess)
        {
            logger.LogInformation("Failed to confirm account for user {UserId}: {Error}", user.Id, confirmUser.Error.Description);
            return ResultT<ResponseDto>.Failure(confirmUser.Error);
        } 
        logger.LogInformation("Account confirmed successfully for user {UserId}", user.Id);
            return ResultT<ResponseDto>.Success(new ResponseDto("Account confirmed successfully"));
    }
}
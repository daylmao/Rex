using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler(
    ILogger<ConfirmAccountCommandHandler> logger,
    IUserRepository userRepository,
    ICodeService codeService
): ICommandHandler<ConfirmAccountCommand, string>
{
    public async Task<ResultT<string>> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogInformation("ConfirmAccountCommand received is null");
            return ResultT<string>.Failure(Error.Failure("400", "Request cannot be null"));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogInformation("User with ID {UserId} not found", request.UserId);
            return ResultT<string>.Failure(Error.NotFound("404", "User not found"));
        }
        
        var confirmUser = await codeService.ConfirmAccountAsync(user.Id, request.Code, cancellationToken);
        if (!confirmUser.IsSuccess)
        {
            logger.LogInformation("Failed to confirm account for user {UserId}: {Error}", user.Id, confirmUser.Error.Description);
            return ResultT<string>.Failure(confirmUser.Error);
        }
        
        logger.LogInformation("Account confirmed successfully for user {UserId}", user.Id);
        return ResultT<string>.Success("Account confirmed successfully");
    }
}
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.InactiveAccount;

public class InactiveAccountCommandHandler(
    ILogger<InactiveAccountCommandHandler> logger,
    IUserRepository userRepository
    ): ICommandHandler<InactiveAccountCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(InactiveAccountCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Inactive account command is null");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! Something went wrong with your request."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "We couldn't find your account"));
        }
        
        if (user.Deleted)
        {
            logger.LogWarning("User {UserId} account is already deactivated.", user.Id);
            return ResultT<ResponseDto>.Failure(Error.Failure("403",
                "Your account is already deactivated. Please contact support to reactivate your account."));
        }
        
        logger.LogWarning("Deactivating user account {UserId}", user.Id);
        await userRepository.DeleteAsync(user, cancellationToken);
        
        return ResultT<ResponseDto>.Success(new ("Your account has been deactivated. We're sorry to see you go!"));
    }
}
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.UpdateUsername;

public class UpdateUsernameCommandHandler(
    ILogger<UpdateUsernameCommandHandler> logger,
    IUserRepository userRepository
) : ICommandHandler<UpdateUsernameCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateUsernameCommand request was null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Invalid request."));
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", $"User with ID {request.UserId} not found."));
        }
        
        var usernameInUse = await userRepository.UserNameInUseAsync(request.UserId, request.Username, cancellationToken);
        if (usernameInUse)
        {
            logger.LogWarning("Username '{Username}' is already in use.", request.Username);
            return ResultT<ResponseDto>.Failure(Error.Conflict("409", $"Username '{request.Username}' is already in use."));
        }
        
        user.UserName = request.Username;
        await userRepository.UpdateAsync(user, cancellationToken);
        
        logger.LogInformation("Username for user {UserId} updated successfully to '{Username}'.", request.UserId, request.Username);
        return ResultT<ResponseDto>.Success(new ResponseDto("Username updated successfully."));
    }
}
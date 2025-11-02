using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.UpdateUsername;

public class UpdateUsernameCommandHandler(
    ILogger<UpdateUsernameCommandHandler> logger,
    IUserRepository userRepository,
    IDistributedCache cache
) : ICommandHandler<UpdateUsernameCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to update username.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! We didn't get the information needed to update your username."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for username update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your account."));
        }

        var usernameInUse =
            await userRepository.UserNameInUseAsync(request.UserId, request.Username, cancellationToken);
        if (usernameInUse)
        {
            logger.LogWarning("Username '{Username}' is already in use.", request.Username);
            return ResultT<ResponseDto>.Failure(Error.Conflict("409",
                "Oops! This username is already taken. Try another one."));
        }

        user.UserName = request.Username;
        await userRepository.UpdateAsync(user, cancellationToken);
        
        await cache.IncrementVersionAsync("user", request.UserId, logger, cancellationToken);

        logger.LogInformation("Username updated successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Great! Your username has been updated successfully."));
    }
}
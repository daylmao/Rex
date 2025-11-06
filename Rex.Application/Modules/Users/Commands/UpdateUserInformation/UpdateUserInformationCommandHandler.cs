using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Users.Commands.UpdateUserInformation;

public class UpdateUserInformationCommandHandler(
    ILogger<UpdateUserInformationCommandHandler> logger,
    IUserRepository userRepository,
    ICloudinaryService cloudinaryService,
    IDistributedCache cache
) : ICommandHandler<UpdateUserInformationCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateUserInformationCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request to update user information.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Oops! We didn't get the information needed to update your profile."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for profile update.");
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find your account."));
        }

        string profilePhoto = "";
        if (request.ProfilePhoto != null)
        {
            logger.LogInformation("Uploading new profile photo.");
            await using var stream = request.ProfilePhoto.OpenReadStream();
            profilePhoto = await cloudinaryService.UploadImageAsync(stream, request.ProfilePhoto.FileName, cancellationToken);
        }

        user.ProfilePhoto = profilePhoto;
        user.FirstName = request.Firstname;
        user.LastName = request.Lastname;
        user.Biography = request.Biography;

        await userRepository.UpdateAsync(user, cancellationToken);
        
        await cache.IncrementVersionAsync("user", request.UserId, logger, cancellationToken);

        logger.LogInformation("User information updated successfully.");
        return ResultT<ResponseDto>.Success(new ResponseDto("Great! Your profile has been updated successfully."));
    }
    
}

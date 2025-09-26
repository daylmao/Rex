using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.User.Commands.UpdateUserInformation;

public class UpdateUserInformationCommandHandler(
    ILogger<UpdateUserInformationCommandHandler> logger,
    IUserRepository userRepository,
    ICloudinaryService cloudinaryService
) : ICommandHandler<UpdateUserInformationCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateUserInformationCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateUserInformationCommandHandler: Request is null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request cannot be null."));
        }

        logger.LogInformation("UpdateUserInformationCommandHandler: Fetching user with ID {UserId}", request.UserId);
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("UpdateUserInformationCommandHandler: User with ID {UserId} not found.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", $"User with ID {request.UserId} not found."));
        }

        string profilePhoto = "";

        if (request.ProfilePhoto != null)
        {
            logger.LogInformation("UpdateUserInformationCommandHandler: Uploading new profile photo for user {UserId}", request.UserId);
            using var stream = request.ProfilePhoto.OpenReadStream();
            profilePhoto = await cloudinaryService.UploadImageAsync(
                stream,
                request.ProfilePhoto.FileName,
                cancellationToken
            );
        }

        user.ProfilePhoto = profilePhoto;
        user.FirstName = request.Firstname;
        user.LastName = request.Lastname;
        user.Biography = request.Biography;

        logger.LogInformation("UpdateUserInformationCommandHandler: Updating user {UserId} information in repository", request.UserId);
        await userRepository.UpdateAsync(user, cancellationToken);

        logger.LogInformation("UpdateUserInformationCommandHandler: User {UserId} updated successfully.", request.UserId);
        return ResultT<ResponseDto>.Success(new ResponseDto("User information updated successfully."));
    }
}

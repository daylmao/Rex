using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler(
    ILogger<UpdateGroupCommandHandler> logger,
    IGroupRepository groupRepository,
    ICloudinaryService cloudinaryService
) : ICommandHandler<UpdateGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateGroupCommand request is null.");
            return ResultT<ResponseDto>.Failure(
                Error.Failure("400", "Oops! No data was provided to update the group."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} not found.", request.GroupId);
            return ResultT<ResponseDto>.Failure(
                Error.Failure("404", "We couldn’t find the group you want to update."));
        }

        if (request.ProfilePhoto is not null)
        {
            logger.LogInformation("Uploading new profile image for group {GroupId}...", request.GroupId);
            await using var stream = request.ProfilePhoto.OpenReadStream();
            var profileUrl =
                await cloudinaryService.UploadImageAsync(stream, request.ProfilePhoto.FileName, cancellationToken);
            group.ProfilePhoto = profileUrl;
            logger.LogInformation("Profile image uploaded successfully for group {GroupId}.", request.GroupId);
        }

        if (request.CoverPhoto is not null)
        {
            logger.LogInformation("Uploading new banner image for group {GroupId}...", request.GroupId);
            await using var stream = request.CoverPhoto.OpenReadStream();
            var coverUrl =
                await cloudinaryService.UploadImageAsync(stream, request.CoverPhoto.FileName, cancellationToken);
            group.CoverPhoto = coverUrl;
            logger.LogInformation("Banner image uploaded successfully for group {GroupId}.", request.GroupId);
        }

        group.Title = request.Title;
        group.Description = request.Description;
        group.Visibility = request.Visibility.ToString();
        group.UpdatedAt = DateTime.UtcNow;

        await groupRepository.UpdateAsync(group, cancellationToken);

        logger.LogInformation("Group with ID {GroupId} updated successfully.", request.GroupId);
        return ResultT<ResponseDto>.Success(
            new ResponseDto("The group has been updated successfully!"));
    }
}
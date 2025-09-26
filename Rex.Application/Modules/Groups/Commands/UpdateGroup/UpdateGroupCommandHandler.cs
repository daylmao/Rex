using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Serilog;

namespace Rex.Application.Modules.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler(
    ILogger<UpdateGroupCommandHandler> logger,
    IGroupRepository groupRepository,
    ICloudinaryService cloudinaryService
    ): ICommandHandler<UpdateGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("UpdateGroupCommand request is null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request is null."));
        }
        
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group with ID {GroupId} not found.", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Group not found."));
        }
        
        if (request.ProfilePhoto != null)
        {
            logger.LogInformation("Uploading new group profile image...");
            await using var stream = request.ProfilePhoto.OpenReadStream();
            var profileUrl = await cloudinaryService.UploadImageAsync(
                stream, 
                request.ProfilePhoto.FileName, 
                cancellationToken);
            group.ProfilePhoto = profileUrl;
            logger.LogInformation("Profile image uploaded successfully.");
        }
        
        if (request.CoverPhoto != null)
        {
            logger.LogInformation("Uploading new group banner image...");
            await using var stream = request.CoverPhoto.OpenReadStream();
            var coverUrl = await cloudinaryService.UploadImageAsync(
                stream, 
                request.CoverPhoto.FileName, 
                cancellationToken);
            group.CoverPhoto = coverUrl;
            logger.LogInformation("Banner image uploaded successfully.");
        }
        
        group.Title = request.Title;
        group.Description = request.Description;
        group.Visibility = request.Visibility.ToString();
        group.UpdatedAt = DateTime.UtcNow;
        
        await groupRepository.UpdateAsync(group, cancellationToken);
        
        logger.LogInformation("Group with ID {GroupId} updated successfully.", request.GroupId);
        return ResultT<ResponseDto>.Success(new ResponseDto("Group updated successfully."));
    }
}
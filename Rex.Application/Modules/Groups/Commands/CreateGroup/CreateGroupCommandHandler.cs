using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;
using File = Rex.Models.File;
using GroupRole = Rex.Enum.GroupRole;

namespace Rex.Application.Modules.Groups.Commands;

public class CreateGroupCommandHandler(
    ILogger<CreateGroupCommandHandler> logger,
    IGroupRepository groupRepository,
    ICloudinaryService cloudinaryService,
    IGroupRoleRepository groupRoleRepository,
    IUserGroupRepository userGroupRepository
    ): ICommandHandler<CreateGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("CreateGroupCommand request is null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "Request is null."));
        }

        string profileUrl = "";
        if (request.ProfilePhoto != null)
        {
            logger.LogInformation("Uploading group profile image...");
            await using var stream = request.ProfilePhoto.OpenReadStream();
            profileUrl = await cloudinaryService.UploadImageAsync(stream, request.ProfilePhoto.FileName, cancellationToken);
            logger.LogInformation("Profile image uploaded successfully.");
        }

        string coverUrl = "";
        if (request.CoverPhoto != null)
        {
            logger.LogInformation("Uploading group banner image...");
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverUrl = await cloudinaryService.UploadImageAsync(stream, request.CoverPhoto.FileName, cancellationToken);
            logger.LogInformation("Banner image uploaded successfully.");
        }

        Group group = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility.ToString(),
            ProfilePhoto = profileUrl,
            CoverPhoto = coverUrl
        };

        await groupRepository.CreateAsync(group, cancellationToken);
        logger.LogInformation("Group entity created successfully.");

        var leaderRole = await groupRoleRepository.GetRoleByNameAsync(GroupRole.Leader, cancellationToken);
        if (leaderRole == null)
        {
            logger.LogError("Group role 'Leader' not found.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Group role 'Leader' not found."));
        }

        UserGroup userGroup = new()
        {
            UserId = request.UserId,
            GroupId = group.Id,
            GroupRoleId = leaderRole.Id,
            Status = RequestStatus.Accepted.ToString(),
            RequestedAt = DateTime.UtcNow
        };

        await userGroupRepository.CreateAsync(userGroup, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("Group created successfully."));

    }
}

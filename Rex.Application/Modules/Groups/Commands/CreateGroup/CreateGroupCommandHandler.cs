using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;
using GroupRole = Rex.Enum.GroupRole;

namespace Rex.Application.Modules.Groups.Commands;

public class CreateGroupCommandHandler(
    ILogger<CreateGroupCommandHandler> logger,
    IGroupRepository groupRepository,
    ICloudinaryService cloudinaryService,
    IGroupRoleRepository groupRoleRepository,
    IUserGroupRepository userGroupRepository
) : ICommandHandler<CreateGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("CreateGroupCommand request is null.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400", "No information received to create the group."));
        }

        string profileUrl = "";
        if (request.ProfilePhoto != null)
        {
            logger.LogInformation("Uploading profile image for group '{GroupTitle}'...", request.Title);
            await using var stream = request.ProfilePhoto.OpenReadStream();
            profileUrl = await cloudinaryService.UploadImageAsync(stream, request.ProfilePhoto.FileName, cancellationToken);
            logger.LogInformation("Profile image uploaded successfully for group '{GroupTitle}'", request.Title);
        }

        string coverUrl = "";
        if (request.CoverPhoto != null)
        {
            logger.LogInformation("Uploading banner image for group '{GroupTitle}'...", request.Title);
            await using var stream = request.CoverPhoto.OpenReadStream();
            coverUrl = await cloudinaryService.UploadImageAsync(stream, request.CoverPhoto.FileName, cancellationToken);
            logger.LogInformation("Banner image uploaded successfully for group '{GroupTitle}'", request.Title);
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
        logger.LogInformation("Group '{GroupTitle}' created successfully with ID {GroupId}", group.Title, group.Id);

        var leaderRole = await groupRoleRepository.GetRoleByNameAsync(GroupRole.Leader, cancellationToken);
        if (leaderRole == null)
        {
            logger.LogError("Group role 'Leader' not found for group '{GroupTitle}'", group.Title);
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "Leader role not found to assign."));
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
        logger.LogInformation("User with ID {UserId} assigned as Leader to group '{GroupTitle}'", request.UserId, group.Title);

        return ResultT<ResponseDto>.Success(new ResponseDto("Group created successfully! You are now the leader of the group."));
    }
}

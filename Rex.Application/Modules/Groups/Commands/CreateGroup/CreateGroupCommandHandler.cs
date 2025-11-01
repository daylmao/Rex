using Microsoft.Extensions.Caching.Distributed;
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
    IUserGroupRepository userGroupRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : ICommandHandler<CreateGroupCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "The user does not exist or could not be found."));
        }

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to create a group but the account is not confirmed.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a group."));
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
        
        await cache.IncrementVersionAsync("groups", request.UserId, logger, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("Group created successfully! You are now the leader of the group."));
    }
}

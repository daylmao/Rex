using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Posts.Commands;

public class CreatePostCommandHandler(
    ILogger<CreatePostCommandHandler> logger,
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IPostRepository postRepository,
    IUserGroupRepository userGroupRepository,
    IUserChallengeRepository userChallengeRepository,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService
) : ICommandHandler<CreatePostCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogWarning("Received empty request for creating a post.");
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "Oops! We couldn't process your request. Please provide the post details."));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with the provided GroupId.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404", "We couldn't find the group you selected."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("No user found with the provided UserId.");
            return ResultT<ResponseDto>.Failure(Error.Failure("404",
                "We couldn't find your account. Please check your details."));
        }

        var isUserInGroup = await userGroupRepository.IsUserInGroupAsync(
            request.UserId, request.GroupId, RequestStatus.Accepted, cancellationToken);

        if (!isUserInGroup)
        {
            logger.LogWarning("User {UserId} is not a member of group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403",
                "You need to be a member of the group to create a post."));
        }

        if (request.ChallengeId.HasValue)
        {
            var challenge = await userChallengeRepository.GetByIdAsync(request.ChallengeId.Value, cancellationToken);
            if (challenge is not null)
            {
                if (challenge.UserId != request.UserId)
                {
                    logger.LogWarning(
                        "User {UserId} attempted to attach challenge {ChallengeId} they are not enrolled in.",
                        request.UserId, request.ChallengeId);

                    return ResultT<ResponseDto>.Failure(Error.Failure("403",
                        "Oops! You can only attach challenges you're enrolled in."));
                }

                challenge.Status = UserChallengeStatus.Completed.ToString();
                await userChallengeRepository.UpdateAsync(challenge, cancellationToken);
            }
        }

        Post post = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            GroupId = request.GroupId,
            UserId = request.UserId,
            ChallengeId = request.ChallengeId
        };

        await postRepository.CreateAsync(post, cancellationToken);
        logger.LogInformation("Post '{PostTitle}' created successfully in group {GroupId} by user {UserId}.",
            request.Title, request.GroupId, request.UserId);
        
        if (request.Files is not null)
        {
            var filesResult = await ProcessFiles.ProcessFilesAsync(logger, request.Files, post.Id, fileRepository,
                entityFileRepository, cloudinaryService, TargetType.Post, cancellationToken);
            
            if (!filesResult.IsSuccess)
                return filesResult;
        }
        
        return ResultT<ResponseDto>.Success(new ResponseDto("Your post has been created!"));
    }
}
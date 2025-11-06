using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Modules.Posts.Commands.CreatePost;
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
    IChallengeRepository challengeRepository,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService,
    IDistributedCache cache
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

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("No user found with UserId {UserId}.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "We couldn't find your account. Please log in again."));
        }
        
        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to create a group but the account is not confirmed.", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a group."));
        }

        var group = await groupRepository.GetGroupByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("No group found with GroupId {GroupId}.", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "We couldn't find the group you're trying to post in."));
        }

        var isUserInGroup = await userGroupRepository.IsUserInGroupAsync(
            request.UserId, request.GroupId, RequestStatus.Accepted, cancellationToken);

        if (!isUserInGroup)
        {
            logger.LogWarning("User {UserId} is not a member of group {GroupId}.", request.UserId, request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.Failure("403",
                "You need to be a member of this group to create posts."));
        }

        if (request.ChallengeId.HasValue)
        {
            var userChallenge = await userChallengeRepository.GetByUserAndChallengeAsync(
                request.UserId, request.ChallengeId.Value, cancellationToken);

            if (userChallenge is null)
            {
                logger.LogWarning("User {UserId} is not enrolled in challenge {ChallengeId}.",
                    request.UserId, request.ChallengeId);
                return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                    "You're not enrolled in this challenge. Please join it first."));
            }

            var belongsToGroup = await challengeRepository.ChallengeBelongsToGroup(
                request.GroupId, userChallenge.ChallengeId, cancellationToken);

            if (!belongsToGroup)
            {
                logger.LogWarning("Challenge {ChallengeId} doesn't belong to group {GroupId}.",
                    userChallenge.ChallengeId, request.GroupId);
                return ResultT<ResponseDto>.Failure(Error.Failure("400",
                    "This challenge doesn't belong to the group you're posting in."));
            }

            if (userChallenge.Status == UserChallengeStatus.Completed.ToString())
            {
                logger.LogWarning("User {UserId} attempted to complete already completed challenge {ChallengeId}.",
                    request.UserId, request.ChallengeId);
                return ResultT<ResponseDto>.Failure(Error.Failure("400",
                    "You've already completed this challenge!"));
            }

            userChallenge.Status = UserChallengeStatus.Completed.ToString();
            await userChallengeRepository.UpdateAsync(userChallenge, cancellationToken);

            logger.LogInformation("User {UserId} completed challenge {ChallengeId}.",
                request.UserId, userChallenge.ChallengeId);
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

        if (request.Files is not null && request.Files.Any())
        {
            var filesResult = await ProcessFiles.ProcessFilesAsync(
                logger, request.Files, post.Id, fileRepository,
                entityFileRepository, cloudinaryService, TargetType.Post, cancellationToken);

            if (!filesResult.IsSuccess)
                return filesResult;
        }
        
        await cache.IncrementVersionAsync("group-posts", request.GroupId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for posts of GroupId: {GroupId}", request.GroupId);
        
        await userGroupRepository.ResetWarningStatus(post.UserId, post.GroupId, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("Your post has been created successfully!"));
    }
}
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.JWT;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Posts.Commands.DeletePost;

public class DeletePostCommandHandler(
    ILogger<DeletePostCommandHandler> logger,
    IUserGroupRepository userGroupRepository,
    IUserRepository userRepository,
    IPostRepository postRepository,
    IGroupRepository groupRepository,
    IDistributedCache cache
) : ICommandHandler<DeletePostCommand, ResponseDto>
{
    public async Task<ResultT<ResponseDto>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found. UserId: {UserId}", request.UserId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "User not found."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("Post not found. PostId: {PostId}", request.PostId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Post not found."));
        }

        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            logger.LogWarning("Group not found. GroupId: {GroupId}", request.GroupId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "Group not found."));
        }

        var userGroup = await userGroupRepository.GetMemberAsync(request.UserId, group.Id, cancellationToken);
        if (userGroup is null)
        {
            logger.LogWarning("User is not a member of the group. UserId: {UserId}, GroupId: {GroupId}",
                request.UserId, group.Id);
            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You are not a member of this group."));
        }

        var role = userGroup.GroupRole.Role;

        if (!CanDeletePost(role, request.UserId, post.UserId))
        {
            logger.LogWarning("User {UserId} with role {Role} attempted to delete post {PostId} without permission.",
                request.UserId, role, post.Id);

            return ResultT<ResponseDto>.Failure(Error.Failure("403", "You can only delete your own posts."));
        }

        await postRepository.DeleteAsync(post, cancellationToken);

        logger.LogInformation("Cache invalidated for posts of GroupId: {GroupId}", group.Id);
        await cache.IncrementVersionAsync("group-posts", group.Id, logger, cancellationToken);

        logger.LogInformation("Post {PostId} deleted by User {UserId} with role {Role}", post.Id, request.UserId, role);

        return ResultT<ResponseDto>.Success(new ResponseDto("Post deleted successfully."));
    }

    private bool CanDeletePost(string role, Guid userId, Guid authorId)
    {
        if (userId == authorId)
            return true;

        return role == GroupRole.Leader.ToString() ||
               role == GroupRole.Mentor.ToString() ||
               role == GroupRole.Moderator.ToString();
    }
}



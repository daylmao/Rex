using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Interfaces.SignalR;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Modules.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler(
    ILogger<CreateCommentCommandHandler> logger,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IPostRepository postRepository,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService,
    ICommentsNotifier commentsNotifier,
    IDistributedCache cache
) : ICommandHandler<CreateCommentCommand, CommentDto>
{
    public async Task<ResultT<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CreateCommentCommand for UserId: {UserId}, PostId: {PostId}",
            request.UserId, request.PostId);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return ResultT<CommentDto>.Failure(Error.NotFound("404", "Oops! The specified user doesn't seem to exist."));

        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
            return ResultT<CommentDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a comment."));

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
            return ResultT<CommentDto>.Failure(Error.NotFound("404", "Hmm, we couldn't find the target post."));

        Comment comment = new Comment
        {
            Description = request.Description,
            PostId = request.PostId,
            UserId = request.UserId,
            IsPinned = false,
            Edited = false
        };

        await commentRepository.CreateAsync(comment, cancellationToken);

        if (request.Files is not null && request.Files.Any())
        {
            var filesResult = await ProcessFiles.ProcessFilesAsync(logger, request.Files, comment.Id,
                fileRepository, entityFileRepository, cloudinaryService, TargetType.Comment, cancellationToken);

            if (!filesResult.IsSuccess)
                return filesResult.Error;
        }

        var files = await fileRepository.GetFilesByTargetIdAsync(comment.Id, TargetType.Comment, cancellationToken);

        CommentDto commentDto = new CommentDto(
            comment.Id,
            comment.PostId,
            comment.UserId,
            comment.Description,
            comment.IsPinned,
            comment.Edited,
            files
        );

        await cache.IncrementVersionAsync("comments", request.PostId, logger, cancellationToken);

        var metadata = new
        {
            PostId = post.Id,
            PostTitle = post.Title,
            CommentId = comment.Id,
            CommentAuthor = $"{user.FirstName} {user.LastName}"
        };

        var notification = new Notification
        {
            Title = "New Comment",
            Description = $"{user.FirstName} {user.LastName} commented on your post '{post.Title}'",
            UserId = user.Id,
            RecipientType = TargetType.User.ToString(),
            RecipientId = post.UserId,
            MetadataJson = JsonSerializer.Serialize(metadata),
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        await commentsNotifier.SendCommentNotification(notification, cancellationToken);

        return ResultT<CommentDto>.Success(commentDto);
    }
}

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.File;
using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;
using Rex.Models;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.SignalR;

namespace Rex.Application.Modules.Comments.Commands.CreateCommentReply;

public class CreateCommentReplyCommandHandler(
    ILogger<CreateCommentReplyCommandHandler> logger,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    ICommentsNotifier commentsNotifier,
    IPostRepository postRepository,
    IEntityFileRepository entityFileRepository,
    IFileRepository fileRepository,
    ICloudinaryService cloudinaryService,
    IDistributedCache cache
) : ICommandHandler<CreateCommentReplyCommand, ReplyDto>
{
    public async Task<ResultT<ReplyDto>> Handle(CreateCommentReplyCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Create reply failed: User with ID '{UserId}' not found.", request.UserId);
            return ResultT<ReplyDto>.Failure(Error.NotFound("404", "User not found."));
        }
        
        var accountConfirmed = await userRepository.ConfirmedAccountAsync(request.UserId, cancellationToken);
        if (!accountConfirmed)
        {
            logger.LogWarning("User with ID {UserId} tried to create a reply but the account is not confirmed.", request.UserId);
            return ResultT<ReplyDto>.Failure(Error.Failure("403", "You need to confirm your account before creating a reply."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("Create reply failed: Post with ID '{PostId}' not found.", request.PostId);
            return ResultT<ReplyDto>.Failure(Error.NotFound("404", "Post not found."));
        }

        var parentComment = await commentRepository.GetByIdAsync(request.ParentCommentId, cancellationToken);
        if (parentComment is null)
        {
            logger.LogWarning("Create reply failed: Parent comment with ID '{ParentCommentId}' not found.", request.ParentCommentId);
            return ResultT<ReplyDto>.Failure(Error.NotFound("404", "Parent comment not found."));
        }

        var userComment = await userRepository.GetUserByCommentIdAsync(request.ParentCommentId, cancellationToken);
        if (userComment is null)
        {
            logger.LogWarning("Failed to notify reply: Original comment's user not found for ParentCommentId '{ParentCommentId}'", request.ParentCommentId);
            return ResultT<ReplyDto>.Failure(Error.NotFound("404", "Original comment's user not found."));
        }

        var reply = new Comment
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            PostId = request.PostId,
            UserId = request.UserId,
            ParentCommentId = request.ParentCommentId,
            IsPinned = false,
            Edited = false
        };

        await commentRepository.CreateAsync(reply, cancellationToken);

        if (request.Files is not null && request.Files.Any())
        {
            var filesResult = await ProcessFiles.ProcessFilesAsync(
                logger, 
                request.Files, 
                reply.Id,
                fileRepository, 
                entityFileRepository, 
                cloudinaryService, 
                TargetType.Comment, 
                cancellationToken
            );

            if (!filesResult.IsSuccess)
            {
                return filesResult.Error;
            }
        }

        var files = await fileRepository.GetFilesByTargetIdAsync(reply.Id, TargetType.Comment, cancellationToken);

        var replyFiles = files.Where(f => f.EntityFiles.Any(e => e.TargetId == reply.Id)).ToList();

        var replyDto = new ReplyDto(
            reply.Id,
            reply.ParentCommentId!.Value,
            reply.Description,
            reply.Edited,
            false,
            new UserCommentDetailsDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.ProfilePhoto
            ),
            reply.CreatedAt,
            replyFiles.Select(c => new FileDetailDto(c.Id,c.Url, c.Type))
        );

        logger.LogInformation(
            "Reply created successfully with ID '{ReplyId}', parent comment '{ParentCommentId}', by user '{UserId}' on post '{PostId}'.",
            reply.Id, reply.ParentCommentId, reply.UserId, reply.PostId
        );
        
        await cache.IncrementVersionAsync("comments", request.ParentCommentId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for replies of ParentCommentId: {ParentCommentId}", request.ParentCommentId);
        
        var metadata = new
        {
            PostId = post.Id,
            PostTitle = post.Title,
            CommentId = reply.ParentCommentId,
            ReplyId = reply.Id,
            ReplyAuthor = $"{user.FirstName} {user.LastName}"
        };

        var notification = new Notification
        {
            Title = "New Reply",
            Description = $"{user.FirstName} {user.LastName} replied to your comment on '{post.Title}'",
            UserId = user.Id,
            RecipientType = "User",
            RecipientId = userComment.Id,
            MetadataJson = JsonSerializer.Serialize(metadata),
            Read = false,
            CreatedAt = DateTime.UtcNow
        };

        await commentsNotifier.SendReplyNotification(notification, cancellationToken);
        
        return ResultT<ReplyDto>.Success(replyDto);
    }
}

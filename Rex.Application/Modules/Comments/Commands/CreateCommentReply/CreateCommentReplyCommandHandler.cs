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

namespace Rex.Application.Modules.Comments.Commands.CreateCommentReply;

public class CreateCommentReplyCommandHandler(
    ILogger<CreateCommentReplyCommandHandler> logger,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IPostRepository postRepository,
    IEntityFileRepository entityFileRepository,
    IFileRepository fileRepository,
    ICloudinaryService cloudinaryService
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

        return ResultT<ReplyDto>.Success(replyDto);
    }
}

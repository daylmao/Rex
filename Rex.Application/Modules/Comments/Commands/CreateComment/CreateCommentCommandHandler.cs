using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
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
    ICloudinaryService cloudinaryService
) : ICommandHandler<CreateCommentCommand, CommentDto>
{
    public async Task<ResultT<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CreateCommentCommand for UserId: {UserId}, PostId: {PostId}",
            request.UserId, request.PostId);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Failed to create comment: User with ID '{UserId}' was not found.", request.UserId);

            return ResultT<CommentDto>.Failure(
                Error.NotFound("404", "Oops! The specified user doesn't seem to exist."));
        }

        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post is null)
        {
            logger.LogWarning("Failed to create comment: Target Post with ID '{PostId}' was not found for user '{UserId}'.",
                request.PostId, request.UserId);

            return ResultT<CommentDto>.Failure(
                Error.NotFound("404", "Hmm, we couldn't find the target post."));
        }

        logger.LogInformation("Creating comment for PostId: {PostId} by UserId: {UserId}", request.PostId, request.UserId);

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
            {
                return filesResult.Error;
            }
        }

        var files = await fileRepository.GetFilesByTargetIdAsync(comment.Id, TargetType.Comment, cancellationToken);
        logger.LogInformation("Comment persisted successfully with ID: {CommentId}", comment.Id);

        CommentDto commentDto = new CommentDto(
            comment.Id,
            comment.PostId,
            comment.UserId,
            comment.Description,
            comment.IsPinned,
            comment.Edited,
            files
        );

        logger.LogInformation("Comment successfully created with ID '{CommentId}' by user '{UserId}' on post '{PostId}'.",
            comment.Id, comment.UserId, comment.PostId);

        return ResultT<CommentDto>.Success(commentDto);
    }
}

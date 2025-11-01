using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Comment;
using Rex.Application.DTOs.File;
using Rex.Application.Helpers;
using Rex.Application.Interfaces;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Enum;

namespace Rex.Application.Modules.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler(
    ILogger<UpdateCommentCommandHandler> logger,
    ICommentRepository commentRepository,
    IFileRepository fileRepository,
    IEntityFileRepository entityFileRepository,
    ICloudinaryService cloudinaryService,
    IDistributedCache cache
) : ICommandHandler<UpdateCommentCommand, CommentUpdatedDto>
{
    public async Task<ResultT<CommentUpdatedDto>> Handle(UpdateCommentCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting update for comment {CommentId}...", request.CommentId);

        var comment = await commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment is null)
            return ResultT<CommentUpdatedDto>.Failure(
                Error.NotFound("404", "We couldn’t find the comment you wanted to update."));

        if (request.FilesToDelete is not null && request.FilesToDelete.Any())
        {
            var filesToDelete =
                await fileRepository.GetFilesByTargetIdsAsync(request.FilesToDelete, TargetType.Comment,
                    cancellationToken);

            foreach (var file in filesToDelete)
            {
                file.Deleted = true;
                file.DeletedAt = DateTime.UtcNow;
                await fileRepository.UpdateAsync(file, cancellationToken);
            }

            logger.LogInformation("Removed {Count} file(s) from comment {CommentId}", filesToDelete.Count(), comment.Id);
        }

        if (request.NewFiles is not null && request.NewFiles.Any())
        {
            var filesResult = await ProcessFiles.ProcessFilesAsync(
                logger, request.NewFiles, comment.Id, fileRepository, entityFileRepository,
                cloudinaryService, TargetType.Comment, cancellationToken);

            if (!filesResult.IsSuccess)
                return filesResult.Error;
        }

        comment.Description = request.Description;
        comment.Edited = true;
        await commentRepository.UpdateAsync(comment, cancellationToken);

        await cache.IncrementVersionAsync("comments", comment.PostId, logger, cancellationToken);
        logger.LogInformation("Cache invalidated for comments of PostId: {PostId}", comment.PostId);
        
        var currentFiles =
            await fileRepository.GetFilesByTargetIdAsync(comment.Id, TargetType.Comment, cancellationToken);

        var dto = new CommentUpdatedDto(
            comment.Id,
            comment.Description,
            comment.IsPinned,
            comment.Edited,
            currentFiles.Select(f => new FileDetailDto(f.Id,f.Url, f.Type))
        );

        logger.LogInformation("Comment {CommentId} updated successfully", comment.Id);

        return ResultT<CommentUpdatedDto>.Success(dto);
    }
}
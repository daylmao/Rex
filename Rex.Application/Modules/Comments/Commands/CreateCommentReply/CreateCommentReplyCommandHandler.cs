using Microsoft.Extensions.Logging;
using Rex.Application.Abstractions.Messages;
using Rex.Application.DTOs.Reply;
using Rex.Application.DTOs.User;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Utilities;
using Rex.Models;

namespace Rex.Application.Modules.Comments.Commands.CreateCommentReply;

public class CreateCommentReplyCommandHandler(
    ILogger<CreateCommentReplyCommandHandler> logger,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IPostRepository postRepository
    ): ICommandHandler<CreateCommentReplyCommand, ReplyDto>
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

        var replyDto = new ReplyDto(
            reply.Id,                      
            reply.ParentCommentId!.Value,   
            reply.Description,
            reply.Edited,
            reply.Replies.Any(),
            new UserCommentDetailsDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.ProfilePhoto),
            reply.CreatedAt
        );

        logger.LogInformation("Reply created successfully with ID '{ReplyId}', parent comment '{ParentCommentId}', by user '{UserId}' on post '{PostId}'.",
            reply.Id, reply.ParentCommentId, reply.UserId, reply.PostId);

        return ResultT<ReplyDto>.Success(replyDto);
    }
}
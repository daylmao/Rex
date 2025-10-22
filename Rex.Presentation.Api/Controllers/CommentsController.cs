using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.Comment;
using Rex.Application.DTOs.Reply;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Comments.Commands.CreateComment;
using Rex.Application.Modules.Comments.Commands.CreateCommentReply;
using Rex.Application.Modules.Comments.Commands.PinComment;
using Rex.Application.Modules.Comments.Commands.UpdateComment;
using Rex.Application.Modules.Comments.Queries.GetCommentReplies;
using Rex.Application.Modules.Comments.Queries.GetCommentsByPostId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class CommentsController(IMediator mediator, IUserClaims userClaims) : ControllerBase
{
    [HttpGet("post/{postId}")]
    [SwaggerOperation(
        Summary = "Get paginated comments by post ID",
        Description = "Returns paginated comments for a given post, including first reply if any"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ResultT<PagedResult<CommentDetailsDto>>> GetCommentsByPostIdAsync(
        [FromRoute] Guid postId, [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCommentsByPostIdQuery(postId, pageNumber, pageSize), cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new comment",
        Description = "Creates a new comment for a specific post by a user"
    )]
    [ProducesResponseType(typeof(ResultT<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<CommentDto>> CreateCommentAsync(
        [FromForm] CreateCommentDto createComment,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new CreateCommentCommand(createComment.PostId, userId, createComment.Description, createComment.Files),
            cancellationToken);
    }

    [HttpPost("reply")]
    [SwaggerOperation(
        Summary = "Create a new reply to a comment",
        Description = "Creates a new reply for a specific parent comment"
    )]
    [ProducesResponseType(typeof(ResultT<ReplyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ReplyDto>> CreateCommentReplyAsync(
        [FromForm] CreateCommentReplyDto createCommentReply,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new CreateCommentReplyCommand(createCommentReply.ParentCommentId, createCommentReply.PostId, userId,
                createCommentReply.Description, createCommentReply.Files), cancellationToken);
    }

    [HttpGet("replies/{parentCommentId}")]
    [SwaggerOperation(
        Summary = "Get replies for a specific comment",
        Description = "Returns paginated replies for a specific parent comment in a post"
    )]
    [ProducesResponseType(typeof(ResultT<PagedResult<ReplyDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<ReplyDto>>> GetRepliesByCommentIdAsync(
        [FromRoute] Guid parentCommentId, [FromQuery] Guid postId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(
            new GetCommentRepliesQuery(
                postId,
                parentCommentId,
                pageNumber,
                pageSize
            ),
            cancellationToken
        );
    }

    [HttpPost("pin")]
    [SwaggerOperation(
        Summary = "Pin or unpin a comment",
        Description = "Pins or unpins a comment on a specific post by a user. Requires post ownership."
    )]
    [ProducesResponseType(typeof(ResultT<CommentUpdatedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultT<CommentUpdatedDto>>> PinCommentAsync(
        [FromBody] PinCommentDto pinComment, CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return Ok(await mediator.Send(
            new PinCommentCommand(pinComment.CommentId, userId, pinComment.PostId, pinComment.Pin), cancellationToken));
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update an existing comment",
        Description = "Update the description of a comment in a post."
    )]
    [ProducesResponseType(typeof(ResultT<CommentUpdatedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultT<CommentUpdatedDto>>> UpdateCommentAsync(
        [FromForm] UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }
}
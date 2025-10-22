using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.Post;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Posts.Commands;
using Rex.Application.Modules.Posts.Commands.DeletePost;
using Rex.Application.Modules.Posts.Queries.GetPostsByGroupId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostsController(IMediator mediator, IUserClaims userClaims) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = " Create a new post",
        Description = "Creates a new post in a specified group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ResultT<ResponseDto>> CreatePostAsync([FromForm] CreatePostDto createPost,
        CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new CreatePostCommand(createPost.GroupId, userId, createPost.ChallengeId, createPost.Title,
                createPost.Description, createPost.Files), cancellationToken);
    }

    [HttpGet("group/{groupId}/user")]
    [SwaggerOperation(
        Summary = "Get posts by group ID",
        Description = "Retrieves a paginated list of posts for a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<PostDetailsDto>>> GetPostsByGroupIdAsync(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(
            new GetPostsByGroupIdQuery(groupId, userId, pageNumber, pageSize),
            cancellationToken
        );
    }

    [HttpDelete("{postId}/user")]
    [SwaggerOperation(
        Summary = "Delete a post",
        Description = "Deletes a post if the user has permission"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<ResponseDto>> DeletePostAsync(
        [FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var userId = userClaims.GetUserId(User);
        return await mediator.Send(new DeletePostCommand(postId, userId), cancellationToken);
    }
}
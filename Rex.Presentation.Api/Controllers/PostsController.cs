using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostsController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new post",
        Description = "Creates a new post in a specified group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> CreatePostAsync(
        [FromForm] CreatePostDto createPost,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new CreatePostCommand(createPost.GroupId, userId, createPost.ChallengeId, createPost.Title,
                createPost.Description, createPost.Files), cancellationToken);
    }

    [HttpGet("group/{groupId}")]
    [SwaggerOperation(
        Summary = "Get posts by group ID",
        Description = "Retrieves a paginated list of posts for a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<PostDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<PostDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<PagedResult<PostDetailsDto>>))]
    public async Task<ResultT<PagedResult<PostDetailsDto>>> GetPostsByGroupIdAsync(
        [FromRoute] Guid groupId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(
            new GetPostsByGroupIdQuery(groupId, userId, pageNumber, pageSize),
            cancellationToken
        );
    }

    [HttpDelete("{postId}")]
    [SwaggerOperation(
        Summary = "Delete a post",
        Description = "Deletes a post if the user has permission"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> DeletePostAsync(
        [FromRoute] Guid postId,
        [FromQuery] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new DeletePostCommand(postId, groupId, userId), cancellationToken);
    }
}
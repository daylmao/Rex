using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.Post;
using Rex.Application.Modules.Posts.Commands;
using Rex.Application.Modules.Posts.Queries.GetPostsByGroupId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostsController(IMediator mediator) : ControllerBase
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
    public async Task<ResultT<ResponseDto>> CreatePostAsync([FromForm] CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("group/{groupId}")]
    [SwaggerOperation(
        Summary = "Get posts by group ID",
        Description = "Retrieves a paginated list of posts for a specific group"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ResultT<PagedResult<PostDetailsDto>>> GetPostsByGroupIdAsync(
        [FromRoute] Guid groupId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(
            new GetPostsByGroupIdQuery(groupId, pageNumber, pageSize),
            cancellationToken
        );
    }
}
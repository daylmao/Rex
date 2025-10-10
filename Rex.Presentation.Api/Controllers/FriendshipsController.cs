using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs.JWT;
using Rex.Application.Modules.Friendships.Commands;
using Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class FriendshipsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a friendship request",
        Description = "Send a friendship request to another user"
        )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> CreateFriendshipRequest([FromBody] CreateFriendshipRequestCommand command , CancellationToken cancellationToken)
    {
        return await mediator.Send(command , cancellationToken);
    }
    
    [HttpPut("status")]
    [SwaggerOperation(
        Summary = "Manage friendship request",
        Description = "Accept or reject a friendship request"
        )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ResultT<ResponseDto>> ManageFriendshipRequest([FromBody] ManageFriendshipRequestCommand command , CancellationToken cancellationToken)
    {
        return await mediator.Send(command , cancellationToken);
    }
}
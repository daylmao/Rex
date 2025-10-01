using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Friendships.Commands;
using Rex.Application.Modules.Friendships.Commands.ManageFriendshipRequest;
using Rex.Application.Utilities;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class FriendshipsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ResultT<ResponseDto>> CreateFriendshipRequest([FromBody] CreateFriendshipRequestCommand command , CancellationToken cancellationToken)
    {
        return await mediator.Send(command , cancellationToken);
    }
    
    [HttpPut("status")]
    public async Task<ResultT<ResponseDto>> ManageFriendshipRequest([FromBody] ManageFriendshipRequestCommand command , CancellationToken cancellationToken)
    {
        return await mediator.Send(command , cancellationToken);
    }
}
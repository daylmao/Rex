using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rex.Application.DTOs;
using Rex.Application.Modules.Reactions.Commands.ToggleLikeCommand;
using Rex.Application.Utilities;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReactionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ResultT<ResponseDto>> ToggleLikeAsync([FromBody] ToggleLikeCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }
}
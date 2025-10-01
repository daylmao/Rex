using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Rex.Application.DTOs;
using Rex.Application.Modules.Posts.Commands;
using Rex.Application.Utilities;

namespace Rex.Presentation.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ResultT<ResponseDto>> CreatePostAsync([FromForm] CreatePostCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }
  
}
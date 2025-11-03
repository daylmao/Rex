using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rex.Application.DTOs.JWT;
using Rex.Application.DTOs.Notification;
using Rex.Application.Interfaces;
using Rex.Application.Modules.Notifications.Commands.MarkNotificationAsRead;
using Rex.Application.Modules.Notifications.Queries.GetNotificationsByUserId;
using Rex.Application.Pagination;
using Rex.Application.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace Rex.Presentation.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[EnableRateLimiting("api-user")]
[Authorize]
[Route("api/v:{version:apiVersion}/[controller]")]
public class NotificationsController(IMediator mediator, IUserClaimService userClaimService) : ControllerBase
{
    [HttpGet("user-notifications")]
    [SwaggerOperation(
        Summary = "Get notifications by user",
        Description = "Retrieves a paginated list of notifications for the authenticated user."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<PagedResult<NotificationDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<PagedResult<NotificationDto>>))]
    public async Task<ResultT<PagedResult<NotificationDto>>> GetNotificationsByUserIdAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var userId = userClaimService.GetUserId(User);
        return await mediator.Send(new GetNotificationsByUserIdCommand(userId, pageNumber, pageSize),
            cancellationToken);
    }

    [HttpPut("{notificationId}/mark-as-read")]
    [SwaggerOperation(
        Summary = "Mark notification as read",
        Description = "Marks a specific notification as read for the authenticated user."
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultT<ResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultT<ResponseDto>))]
    public async Task<ResultT<ResponseDto>> MarkNotificationAsReadAsync(
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new MarkNotificationAsReadCommand(notificationId), cancellationToken);
    }
}

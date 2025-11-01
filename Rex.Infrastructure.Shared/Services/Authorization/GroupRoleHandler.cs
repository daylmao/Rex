using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rex.Application.Interfaces;

namespace Rex.Infrastructure.Shared.Services.Authorization;

public class GroupRoleHandler(
    ILogger<GroupRoleHandler> logger,
    IUserInGroupService userInGroupService,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<GroupRoleRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GroupRoleRequirement requirement)
    {
        var claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        logger.LogInformation("Claims in user: {@Claims}", claims);

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            logger.LogWarning("HTTP context is null.");
            context.Fail(); 
            return;
        }

        var cancellationToken = httpContext.RequestAborted;

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            logger.LogWarning("User ID claim is missing or invalid.");
            context.Fail();
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("groupId", out var groupIdValue) ||
            groupIdValue is null || !Guid.TryParse(groupIdValue.ToString(), out var groupId))
        {
            logger.LogWarning("Group ID route value is missing or invalid.");
            context.Fail();
            return;
        }

        var userRoleResult = await userInGroupService.GetUserRoleInGroupAsync(userId, groupId, cancellationToken);

        if (!userRoleResult.IsSuccess)
        {
            logger.LogWarning("Failed to retrieve user role in group: {Error}", userRoleResult.Error);
            context.Fail();
            return;
        }

        var userRole = userRoleResult.Value;

        if (!requirement.AllowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "User {UserId} does not have permission to access this resource.",
                userId
            );
            context.Fail();
            return;
        }

        logger.LogInformation(
            "User {UserId} authorized successfully for this request.",
            userId
        );

        context.Succeed(requirement);
    }
}
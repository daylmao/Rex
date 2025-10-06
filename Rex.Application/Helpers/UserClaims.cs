using System.Security.Claims;

namespace Rex.Application.Helpers;

public static class UserClaims
{
    public static Guid? GetUserId(ClaimsPrincipal user)
    {
        if (user == null) return null;

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return null;

        return Guid.TryParse(claim.Value, out var id) ? id : null;
    }
}
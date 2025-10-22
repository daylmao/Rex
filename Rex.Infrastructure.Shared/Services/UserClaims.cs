using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rex.Application.Interfaces;

namespace Rex.Infrastructure.Shared.Services;

public class UserClaims(): IUserClaims
{
    public Guid GetUserId(ClaimsPrincipal user)
    {
        if (user == null) 
            throw new UnauthorizedAccessException("You are not authenticated. Please log in.");

        var claim = user.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim == null || !Guid.TryParse(claim.Value, out var id))
            throw new UnauthorizedAccessException("Unable to identify your user. Please try logging in again.");

        return id;
    }
}
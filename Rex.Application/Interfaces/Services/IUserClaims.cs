using System.Security.Claims;

namespace Rex.Application.Interfaces;

public interface IUserClaims
{
    Guid GetUserId(ClaimsPrincipal user);
}
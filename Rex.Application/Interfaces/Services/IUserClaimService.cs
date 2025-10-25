using System.Security.Claims;

namespace Rex.Application.Interfaces;

public interface IUserClaimService
{
    Guid GetUserId(ClaimsPrincipal user);
}
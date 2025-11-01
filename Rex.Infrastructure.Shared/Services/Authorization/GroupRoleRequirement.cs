using Microsoft.AspNetCore.Authorization;
using Rex.Enum;

namespace Rex.Infrastructure.Shared.Services.Authorization;

public class GroupRoleRequirement(params GroupRole[] requiredRoles) : IAuthorizationRequirement
{
    public IReadOnlyCollection<string> AllowedRoles { get; } = requiredRoles.Select(r => r.ToString()).ToArray();
}

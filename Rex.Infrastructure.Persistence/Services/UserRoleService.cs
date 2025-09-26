using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;
using UserRole = Rex.Enum.UserRole;

namespace Rex.Infrastructure.Persistence.Services;

public class UserRoleService(RexContext context): IUserRoleService
{
    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = new List<string>();
        
        var userRole = await context.Set<User>()
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(c => c.Role.Role)
            .ToListAsync(cancellationToken);


        if (userRole is not null)
        {
            foreach (var role in userRole)
            {
                roles.Add(role);
            }
        }
        
        if (roles.Count is 0)
            roles.Add(UserRole.User.ToString());
        
        return roles;
    }
}
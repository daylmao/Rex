using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserRoleRepository(RexContext context): GenericRepository<UserRole>(context), IUserRoleRepository
{
    
}
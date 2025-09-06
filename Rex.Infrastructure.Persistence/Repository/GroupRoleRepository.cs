using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;

namespace Rex.Infrastructure.Persistence.Repository;

public class GroupRoleRepository(RexContext context): GenericRepository<Models.GroupRole>(context), IGroupRoleRepository
{
    
}
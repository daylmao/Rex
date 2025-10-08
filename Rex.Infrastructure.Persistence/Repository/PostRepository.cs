using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class PostRepository(RexContext context): GenericRepository<Post>(context), IPostRepository
{
    public async Task<PagedResult<Post>> GetPostsByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Post>()
            .AsNoTracking()
            .Where(p => p.GroupId == groupId);
        
        var total = await query.CountAsync(cancellationToken);
        
        var posts = await query
            .Where(p => p.GroupId == groupId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page -1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Post>(posts, total, page, size);
    }
        
}
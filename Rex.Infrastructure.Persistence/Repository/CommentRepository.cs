using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class CommentRepository(RexContext context): GenericRepository<Comment>(context), ICommentRepository
{
    public async Task<PagedResult<Comment>> GetCommentsPaginatedByPostIdAsync(Guid postId, int page, int size, 
        CancellationToken cancellationToken)
    {
        var query = context.Set<Comment>()
            .AsNoTracking()
            .Where(c => c.PostId == postId);
        
        var total = await query.CountAsync(cancellationToken);
        
        var comments = await query
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Comment>(comments, total, page, size);
    }

    public async Task<PagedResult<Comment>> GetCommentsRepliedPaginatedByPostIdAsync(Guid postId, int page, int size,
        Guid parentCommentId, CancellationToken cancellationToken)
    {
        var query = context.Set<Comment>()
            .AsNoTracking()
            .Where(c => c.PostId == postId && c.ParentCommentId == parentCommentId);
        
        var total = await query.CountAsync(cancellationToken);
        
        var comments = await query
            .Where(c => c.PostId == postId && c.ParentCommentId == parentCommentId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Comment>(comments, total, page, size);

    }

    public async Task<int> GetCommentsCountByPostIdAsync(Guid postId, CancellationToken cancellationToken) =>
        await context.Set<Comment>()
            .AsNoTracking()
            .Where(f => f.PostId == postId)
            .CountAsync(cancellationToken);
}
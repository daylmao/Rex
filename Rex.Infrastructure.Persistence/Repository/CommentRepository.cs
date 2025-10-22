using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class CommentRepository(RexContext context) : GenericRepository<Comment>(context), ICommentRepository
{
    public async Task<PagedResult<Comment>> GetCommentsPaginatedByPostIdAsync(
        Guid postId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Comment>()
            .AsNoTracking()
            .Where(c => c.PostId == postId && c.ParentCommentId == null);

        var total = await query.CountAsync(cancellationToken);

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(c => new Comment
            {
                Id = c.Id,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                IsPinned = c.IsPinned,
                Edited = c.Edited,
                PostId = c.PostId,
                UserId = c.UserId,
                ParentCommentId = null,
                User = c.User,
                Replies = c.Replies
                    .OrderBy(r => r.CreatedAt)
                    .Take(1)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>(comments, total, page, size);
    }


    public async Task<PagedResult<Comment>> GetCommentsRepliedPaginatedByParentCommentIdAsync(Guid postId, int page, int size,
        Guid parentCommentId, CancellationToken cancellationToken)
    {
        var query = context.Set<Comment>()
            .AsNoTracking()
            .Include(c => c.Replies)
            .Where(c => c.PostId == postId && c.ParentCommentId == parentCommentId);

        var total = await query.CountAsync(cancellationToken);

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Select(c => new Comment
            {
                Id = c.Id,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                IsPinned = c.IsPinned,
                Edited = c.Edited,
                PostId = c.PostId,
                UserId = c.UserId,
                ParentCommentId = c.ParentCommentId,
                User = c.User,
                Replies = c.Replies
                    .OrderBy(r => r.CreatedAt)
                    .Take(1)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>(comments, total, page, size);
    }

    public async Task<int> GetCommentsCountByPostIdAsync(Guid postId, CancellationToken cancellationToken) =>
        await context.Set<Comment>()
            .AsNoTracking()
            .Where(f => f.PostId == postId)
            .CountAsync(cancellationToken);

    public async Task<int>
        GetCommentsCountByPostIdAsync(IEnumerable<Guid> postId, CancellationToken cancellationToken) =>
        await context.Set<Comment>()
            .AsNoTracking()
            .Where(c => postId.Contains(c.PostId))
            .CountAsync(cancellationToken);

    public async Task<Dictionary<Guid, int>> GetCommentsCountByPostIdsAsync(IEnumerable<Guid> postIds,
        CancellationToken cancellationToken) =>
        await context.Set<Comment>()
            .Where(c => postIds.Contains(c.PostId))
            .AsNoTracking()
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.PostId, g => g.Count, cancellationToken);
}
using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface ICommentRepository: IGenericRepository<Comment>
{
    Task<PagedResult<Comment>> GetCommentsPaginatedByPostIdAsync(Guid postId, int page, int size,
        CancellationToken cancellationToken);

    Task<PagedResult<Comment>> GetCommentsRepliedPaginatedByPostIdAsync(Guid postId, int page, int size,
        Guid parentCommentId, CancellationToken cancellationToken);
    
    Task<int> GetCommentsCountByPostIdAsync(Guid postId, CancellationToken cancellationToken);
}
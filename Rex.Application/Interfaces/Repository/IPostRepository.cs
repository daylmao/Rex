using Rex.Application.Pagination;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IPostRepository: IGenericRepository<Post>
{
    Task<PagedResult<Post>> GetPostsByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken);
    
}
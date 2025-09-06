using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Models;
using File = Rex.Models.File;

namespace Rex.Application.Interfaces.Repository;

public interface IEntityFileRepository: IGenericRepository<EntityFile>
{
    Task<bool> ExistsAsync(Guid targetId, Guid fileId, TargetType targetType, CancellationToken cancellationToken);
    
    Task<int> CountByTargetIdAsync(Guid targetId, TargetType targetType, CancellationToken cancellationToken);

}
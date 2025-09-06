using Rex.Application.Pagination;
using Rex.Enum;
using File = Rex.Models.File;

namespace Rex.Application.Interfaces.Repository;

public interface IFileRepository: IGenericRepository<File>
{
    Task<PagedResult<File>> GetFilesPaginatedByEntityAndTypeAsync(Guid targetId, TargetType targetType, int page,
        int size, FileType type, CancellationToken cancellationToken);

    Task<PagedResult<File>> GetFilesPaginatedByEntityAsync(Guid targetId, TargetType targetType, int page,
        int size, CancellationToken cancellationToken);
}
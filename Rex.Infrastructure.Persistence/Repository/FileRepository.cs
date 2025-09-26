using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using File = Rex.Models.File;

namespace Rex.Infrastructure.Persistence.Repository;

public class FileRepository(RexContext context): GenericRepository<File>(context), IFileRepository
{
    public async Task<PagedResult<File>> GetFilesPaginatedByEntityAndTypeAsync(Guid targetId, TargetType targetType, int page, 
        int size, FileType type, CancellationToken cancellationToken)
    {
        var total = await context.Set<File>()
            .AsNoTracking()
            .Where(f => f.Type == type.ToString() &&
                        f.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString()))
            .CountAsync(cancellationToken);
        
        var files = await context.Set<File>()
            .AsNoTracking()
            .Where(f => f.Type == type.ToString() &&
                        f.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString()))
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<File>(files, total, page, size);
    }

    public async Task<PagedResult<File>> GetFilesPaginatedByEntityAsync(Guid targetId, TargetType targetType, int page, 
        int size, CancellationToken cancellationToken)
    {
        var total = await context.Set<File>()
            .AsNoTracking()
            .Where(t => t.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString()))
            .CountAsync(cancellationToken);
        
        var files = await context.Set<File>()
            .Where(t => t.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString()))
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<File>(files, total, page, size);
    }

    public async Task<File> GetFileByEntityAndTypeAsync(Guid targetId, TargetType targetType, CancellationToken cancellationToken) =>
        await context.Set<File>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.EntityFiles.Any(c => c.TargetId == targetId 
                                                             && c.TargetType == targetType.ToString()), cancellationToken);
}
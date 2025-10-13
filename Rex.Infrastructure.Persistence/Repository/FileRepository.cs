using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using File = Rex.Models.File;

namespace Rex.Infrastructure.Persistence.Repository;

public class FileRepository(RexContext context) : GenericRepository<File>(context), IFileRepository
{
    public async Task<IEnumerable<File>> GetFilesByEntityAndTypeAsync(Guid targetId, TargetType targetType, int page,
        int size, FileType type, CancellationToken cancellationToken) =>
        await context.Set<File>()
            .AsNoTracking()
            .Where(f => f.Type == type.ToString() &&
                        f.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString())
            ).ToListAsync(cancellationToken);


    public async Task<IEnumerable<File>> GetFilesByEntityAsync(Guid targetId, TargetType targetType, int page,
        int size, CancellationToken cancellationToken) =>
        await context.Set<File>()
            .AsNoTracking()
            .Where(t => t.EntityFiles.Any(c => c.TargetId == targetId && c.TargetType == targetType.ToString()))
            .ToListAsync(cancellationToken);


    public async Task<File> GetFileByEntityAndTypeAsync(Guid targetId, TargetType targetType,
        CancellationToken cancellationToken) =>
        await context.Set<File>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.EntityFiles.Any(c => c.TargetId == targetId
                                                             && c.TargetType == targetType.ToString()),
                cancellationToken);
    
    public async Task<IEnumerable<File>> GetFilesByTargetIdsAsync(IEnumerable<Guid> ids, TargetType targetType, CancellationToken ct)
    {
        return await context.File
            .Include(f => f.EntityFiles)
            .Select(c => new File
            {
                Id = c.Id,
                Url = c.Url,
                Type = c.Type,
                EntityFiles = c.EntityFiles.Where(e => ids.Contains(e.TargetId) && e.TargetType == targetType.ToString()).ToList()
            })
            .ToListAsync(ct);
    }

}
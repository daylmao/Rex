using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class EntityFileRepository(RexContext context): GenericRepository<EntityFile>(context), IEntityFileRepository
{
    public async Task<bool> ExistsAsync(Guid targetId, Guid fileId, TargetType targetType, CancellationToken cancellationToken) =>
        await ValidateAsync(e => e.TargetId == targetId && e.FileId == fileId && e.TargetType == targetType.ToString(), 
            cancellationToken);

    public async Task<int> CountByTargetIdAsync(Guid targetId, TargetType targetType,
        CancellationToken cancellationToken) =>
        await context.Set<EntityFile>()
            .Where(e => e.TargetId == targetId && e.TargetType == targetType.ToString())
            .CountAsync(cancellationToken);
}
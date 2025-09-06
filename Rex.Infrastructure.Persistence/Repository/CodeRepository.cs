using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class CodeRepository(RexContext context): GenericRepository<Code>(context), ICodeRepository
{
    public async Task CreateCodeAsync(Code code, CancellationToken cancellationToken)
    {
        await context.Set<Code>().AddAsync(code, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task<Code> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Set<Code>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<bool> IsCodeValid(Code code, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Value == code.Value && c.Expiration > DateTime.UtcNow && !c.Used, cancellationToken);

    public async Task MarkCodeAsUsedAsync(string code, CancellationToken cancellationToken)
    {
        var userCode = await context.Set<Code>()
            .FirstOrDefaultAsync(c => c.Value == code, cancellationToken);

        if (userCode != null)
        {
            userCode.Used = true;
            await SaveAsync(cancellationToken);
        }
    }

    public async Task<bool> IsCodeUsedAsync(Guid code, CancellationToken cancellationToken) =>
         await ValidateAsync( c=> c.Id == code && c.Used, cancellationToken);
    
}
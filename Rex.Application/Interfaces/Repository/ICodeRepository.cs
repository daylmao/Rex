using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface ICodeRepository: IGenericRepository<Code>
{
    Task CreateCodeAsync(Code code, CancellationToken cancellationToken);
    
    Task<Code> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<bool> IsCodeValid(Code code, CancellationToken cancellationToken);
    
    Task MarkCodeAsUsedAsync(string code, CancellationToken cancellationToken);
    
    Task<bool> IsCodeUsedAsync(Guid code, CancellationToken cancellationToken);
}
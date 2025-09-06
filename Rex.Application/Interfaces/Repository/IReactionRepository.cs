using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository;

public interface IReactionRepository: IGenericRepository<Reaction>
{
    Task<int> GetReactionCountByPostIdAsync(Guid postId, ReactionTargetType targetType, CancellationToken cancellationToken);
    
    Task<int> GetReactionCountByCommentIdAsync(Guid commentId, ReactionTargetType targetType, CancellationToken cancellationToken);

}
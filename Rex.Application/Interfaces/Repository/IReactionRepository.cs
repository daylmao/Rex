using Rex.Enum;
using Rex.Models;

namespace Rex.Application.Interfaces.Repository
{
    /// <summary>
    /// Defines methods to interact with reactions in the database.
    /// </summary>
    public interface IReactionRepository : IGenericRepository<Reaction>
    {
        Task<int> CountLikesAsync(Guid targetId, ReactionTargetType targetType, CancellationToken cancellationToken);
        
        Task<Reaction> HasLikedAsync(Guid targetId, Guid userId, ReactionTargetType targetType,
            CancellationToken cancellationToken);
    }
}
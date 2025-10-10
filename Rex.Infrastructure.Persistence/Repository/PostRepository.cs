using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class PostRepository(RexContext context) : GenericRepository<Post>(context), IPostRepository
{
    public async Task<PagedResult<Post>> GetPostsByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Post>()
            .AsNoTracking()
            .Where(p => p.GroupId == groupId)
            .Select(p => new Post
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                GroupId = p.GroupId,
                ChallengeId = p.ChallengeId,
                UserId = p.UserId,
                Challenge = p.Challenge,
                User = new User
                {
                    Id = p.User.Id,
                    ProfilePhoto = p.User.ProfilePhoto,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                    UserChallenges = p.User.UserChallenges
                }
            });

        var total = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Post>(posts, total, page, size);
    }
}
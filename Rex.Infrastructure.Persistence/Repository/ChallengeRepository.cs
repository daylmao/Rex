using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;

namespace Rex.Infrastructure.Persistence.Repository;

public class ChallengeRepository(RexContext context) : GenericRepository<Challenge>(context), IChallengeRepository
{
    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByGroupIdAndStatusAsync(Guid groupId, int page,
        int size, ChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Challenge>()
            .AsNoTracking()
            .Where(g => g.GroupId == groupId && g.Status == status.ToString());

        var total = await query.CountAsync(cancellationToken);

        var challenges = await query
            .Where(g => g.GroupId == groupId && g.Status == status.ToString())
            .Select(g => new Challenge
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Status = g.Status,
                CreatedAt = g.CreatedAt,
                Duration = (g.CreatedAt + g.Duration) - DateTime.UtcNow,
                Group = new Group
                {
                    Id = g.Group.Id,
                    Title = g.Group.Title,
                    ProfilePhoto = g.Group.ProfilePhoto,
                    CoverPhoto = g.Group.CoverPhoto,
                    Description = g.Group.Description,
                    Visibility = g.Group.Visibility
                },
                UserChallenges = g.UserChallenges
                    .Select(uc => new UserChallenge
                    {
                        Id = uc.Id,
                        Status = uc.Status,
                        User = new User
                        {
                            Id = uc.User.Id,
                            ProfilePhoto = uc.User.ProfilePhoto
                        }
                    })
                    .ToList()
            })
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByUserParticipationGroupAndStatusAsync(Guid userId,
        int page, int size,
        Guid groupId, UserChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Challenge>()
            .AsNoTracking()
            .Where(c => c.Status == status.ToString() && c.GroupId == groupId &&
                        c.UserChallenges.Any(g => g.UserId == userId));

        var total = await query.CountAsync(cancellationToken);

        var challenges = await query
            .Where(c => c.Status == status.ToString() && c.GroupId == groupId &&
                        c.UserChallenges.Any(g => g.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<PagedResult<Challenge>> GetChallengesPaginatedByUserIdAndStatusAsync(Guid userId, int page,
        int size,
        UserChallengeStatus status,
        CancellationToken cancellationToken)
    {
        var query = context.Set<Challenge>()
            .AsNoTracking()
            .Where(c => c.UserChallenges.Any(c => c.UserId == userId && c.Status == status.ToString()));

        var total = await query.CountAsync(cancellationToken);

        var challenges = await query
            .Select(c => new Challenge
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                Duration = (c.CreatedAt + c.Duration) - DateTime.UtcNow,
                Group = new Group
                {
                    Id = c.Group.Id,
                    Title = c.Group.Title,
                    ProfilePhoto = c.Group.ProfilePhoto
                },
                UserChallenges = c.UserChallenges
                    .Select(uc => new UserChallenge
                    {
                        Status = uc.Status,
                        UserId = uc.UserId,
                        User = new User
                        {
                            ProfilePhoto = uc.User.ProfilePhoto
                        }
                    }).ToList()
            })
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Challenge>(challenges, total, page, size);
    }

    public async Task<bool> UserAlreadyJoined(Guid userId, Guid challengeId, CancellationToken cancellationToken) =>
        await ValidateAsync(
            c => c.UserChallenges.Any(a =>
                a.UserId == userId && a.ChallengeId == challengeId &&
                a.Status == UserChallengeStatus.Enrolled.ToString()), cancellationToken);

    public async Task<bool> ChallengeBelongsToGroup(Guid groupId, Guid challengeId, CancellationToken cancellationToken) =>
        await ValidateAsync(c => c.Id == challengeId && c.GroupId == groupId, cancellationToken);
}
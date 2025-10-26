using Microsoft.EntityFrameworkCore;
using Rex.Application.Interfaces.Repository;
using Rex.Application.Pagination;
using Rex.Enum;
using Rex.Infrastructure.Persistence.Context;
using Rex.Models;
using GroupRole = Rex.Enum.GroupRole;

namespace Rex.Infrastructure.Persistence.Repository;

public class UserRepository(RexContext context) : GenericRepository<User>(context), IUserRepository
{
    public async Task<bool> ConfirmedAccountAsync(Guid id, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Id == id && u.ConfirmedAccount == true, cancellationToken);

    public async Task<bool> UserNameInUseAsync(Guid userId, string userName, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Id != userId && u.UserName == userName, cancellationToken);

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> EmailInUseAsync(string email, Guid userId, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email && u.Id != userId, cancellationToken);

    public async Task<bool> EmailInUseByYouAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email && u.Id == userId, cancellationToken);

    public async Task UpdatePasswordAsync(User user, string newPassword, CancellationToken cancellationToken)
    {
        user.Password = newPassword;
        context.Set<User>().Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> UserNameExistAsync(string userName, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.UserName == userName, cancellationToken);

    public async Task<PagedResult<User>> GetUsersByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<User>()
            .AsNoTracking()
            .Where(c => c.UserGroups.Any(ug => ug.GroupId == groupId));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Where(c => c.UserGroups.Any(ug => ug.GroupId == groupId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(users, total, page, size);
    }

    public async Task<PagedResult<User>> GetUsersByNameOrLastnameAsync(Guid groupId, string searchTerm, int page,
        int size, CancellationToken cancellationToken)
    {
        var query = context.Set<User>()
            .AsNoTracking()
            .Where(u => u.FirstName.Contains(searchTerm) ||
                        u.LastName.Contains(searchTerm) && u.UserGroups.Any(ug => ug.GroupId == groupId));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Where(u => u.FirstName.Contains(searchTerm) ||
                        u.LastName.Contains(searchTerm) && u.UserGroups.Any(ug => ug.GroupId == groupId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(users, total, page, size);
    }

    public async Task<PagedResult<User>> GetAdministrativeMembersByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Role.Role == GroupRole.Leader.ToString()
                        || u.Role.Role == GroupRole.Moderator.ToString()
                        || u.Role.Role == GroupRole.Mentor.ToString()
                        && u.UserGroups.Any(ug => ug.GroupId == groupId));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Where(u => u.Role.Role == GroupRole.Leader.ToString()
                        || u.Role.Role == GroupRole.Moderator.ToString()
                        || u.Role.Role == GroupRole.Mentor.ToString()
                        && u.UserGroups.Any(ug => ug.GroupId == groupId))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(users, total, page, size);
    }

    public async Task<PagedResult<User>> GetPendingRequestsByGroupIdAsync(Guid groupId, int page, int size,
        CancellationToken cancellationToken)
    {
        var query = context.Set<User>()
            .Where(u => u.UserGroups.Any(g => g.GroupId == groupId && g.Status == RequestStatus.Pending.ToString()));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Where(u => u.UserGroups.Any(g => g.GroupId == groupId && g.Status == RequestStatus.Pending.ToString()))
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(users, total, page, size);
    }


    public async Task<User> GetUserDetailsAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .Where(c => c.Id == id)
            .Select(c => new User
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                UserName = c.UserName,
                Email = c.Email,
                ProfilePhoto = c.ProfilePhoto,
                CoverPhoto = c.CoverPhoto,
                Gender = c.Gender,
                Biography = c.Biography,
                LastLoginAt = c.LastLoginAt,
                Role = c.Role,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                ConfirmedAccount = c.ConfirmedAccount,
                UserGroups = c.UserGroups,
                Reactions = c.Reactions,
                UserChallenges = c.UserChallenges
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task UpdateLastTimeConnectionAsync(Guid userId, bool isActive, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .Where(uc => uc.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(uc => uc.LastConnection, DateTime.UtcNow)
                .SetProperty(uc => uc.IsActive, isActive), cancellationToken);

    public async Task<User> GetUserByCommentIdAsync(Guid parentCommentId, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .Where(c => c.Comments.Any(p => p.ParentCommentId == parentCommentId))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<User> GetByGitHubIdAsync(string githubId, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .FirstOrDefaultAsync(u => u.GitHubId == githubId, cancellationToken);
}
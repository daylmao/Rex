namespace Rex.Application.Interfaces;

/// <summary>
/// Service responsible for managing and retrieving user roles.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Gets the list of role names assigned to a specific user.
    /// </summary>
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
}
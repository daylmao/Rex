using Rex.Enum;

namespace Rex.Models;

public sealed class User : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? ProfilePhoto { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Biography { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool ConfirmedAccount { get; set; } = false;
    public string? Status { get; set; }
    public DateTime? LastConnection { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? RoleId { get; set; }
    public string? GitHubId { get; set; }
    
    public UserRole? Role { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Code> Codes { get; set; } = new List<Code>();
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Reaction>? Reactions { get; set; }
    public ICollection<UserGroup>? UserGroups { get; set; }
    public ICollection<UserChat>? UserChats { get; set; }
    public ICollection<UserChallenge>? UserChallenges { get; set; }
    public ICollection<Notification>? SentNotifications { get; set; }
    public ICollection<Notification>? ReceivedNotifications { get; set; }
    public ICollection<FriendShip>? SentFriendRequests { get; set; }
    public ICollection<FriendShip>? ReceivedFriendRequests { get; set; }
}
using Rex.Enum;

namespace Rex.Models;

public sealed class User : AuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ProfilePhoto { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Biography { get; set; }
    public string Gender { get; set; }
    public DateTime Birthday { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool ConfirmedAccount { get; set; } = false;
    public string Status { get; set; }
    public Guid? RoleId { get; set; }
    
    public UserRole Role { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<Code> Codes { get; set; }
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Reaction>? Reactions { get; set; }
    public ICollection<UserGroup>? UserGroups { get; set; }
    public ICollection<UserChat>? UserChats { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public ICollection<FriendShip>? SentFriendRequests { get; set; }
    public ICollection<FriendShip>? ReceivedFriendRequests { get; set; }

}
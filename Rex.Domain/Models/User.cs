using Rex.Enum;

namespace Rex.Models;

public class User : AuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ProfilePhoto { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Biography { get; set; }
    public Gender Gender { get; set; }
    public DateTime Birthday { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool ConfirmedAccount { get; set; } = false;
    public UserStatus Status { get; set; }
    public Guid RoleId { get; set; }
}
namespace Rex.Models;

public sealed class UserGroup: AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public Guid GroupRoleId { get; set; }
    public string? Status { get; set; }
    public DateTime? LastWarningAt { get; set; }
    public bool HasBeenWarned { get; set; }
    public DateTime RequestedAt { get; set; }
    public User User { get; set; }
    public Group Group { get; set; }
    public GroupRole GroupRole { get; set; }
}
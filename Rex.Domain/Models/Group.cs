using Rex.Enum;

namespace Rex.Models;

public sealed class Group: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; }
    public string ProfilePhoto { get; set; }
    public string? CoverPhoto { get; set; }
    public ICollection<Post>? Posts { get; set; }
    public ICollection<UserGroup> UserGroups { get; set; }
    public ICollection<Challenge>? Challenges { get; set; }
}
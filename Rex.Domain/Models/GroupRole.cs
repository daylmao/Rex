using Rex.Enum;

namespace Rex.Models;

public sealed class GroupRole: AuditableEntity
{
    public string Role { get; set; }
    public ICollection<UserGroup> UserGroups { get; set; }
}
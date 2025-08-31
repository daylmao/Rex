using Rex.Enum;

namespace Rex.Models;

public class GroupRole: AuditableEntity
{
    public Enum.GroupRole Role { get; set; }
}
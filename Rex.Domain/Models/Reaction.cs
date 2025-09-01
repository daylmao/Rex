using Rex.Enum;

namespace Rex.Models;

public sealed class Reaction: AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }

    public string TargetType { get; set; }
    public User User { get; set; }

}
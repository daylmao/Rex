using Rex.Enum;

namespace Rex.Models;

public class Reaction: AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid TargetId { get; set; }
    public ReactionTargetType TargetType { get; set; }
    public bool Impulse { get; set; } = false;
}
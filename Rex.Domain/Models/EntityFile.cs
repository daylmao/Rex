using Rex.Enum;

namespace Rex.Models;

public class EntityFile: AuditableEntity
{
    public Guid FileId { get; set; }
    public Guid TargetId { get; set; }
    public TargetType TargetType { get; set; }
}
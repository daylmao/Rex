using Rex.Enum;

namespace Rex.Models;

public sealed class EntityFile: AuditableEntity
{
    public Guid FileId { get; set; }
    public Guid TargetId { get; set; }
    public string TargetType { get; set; }
    
    public File File { get; set; }
}
using Rex.Enum;

namespace Rex.Models;

public sealed class File: AuditableEntity
{
    public string Url { get; set; }
    public string Type { get; set; }
    public DateTime UploadedAt { get; set; }
    
    public ICollection<EntityFile>? EntityFiles { get; set; }
}
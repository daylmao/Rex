using Rex.Enum;

namespace Rex.Models;

public class File: AuditableEntity
{
    public string Url { get; set; }
    public FileType Type { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}
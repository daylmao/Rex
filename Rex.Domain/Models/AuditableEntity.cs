namespace Rex.Models;

public class AuditableEntity : EntityBase
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool Deleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
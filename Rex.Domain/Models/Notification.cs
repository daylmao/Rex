namespace Rex.Models;

public sealed class Notification: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public bool Read { get; set; } = false;

    public string RecipientType { get; set; }
    public Guid RecipientId { get; set; }
    public string MetadataJson { get; set; }

    public User User { get; set; }
}
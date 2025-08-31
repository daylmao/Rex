namespace Rex.Models;

public class Notification: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public bool Read { get; set; } = false;
}
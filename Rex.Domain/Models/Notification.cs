namespace Rex.Models;

public sealed class Notification: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public Guid RecipientId { get; set; }
    public bool Read { get; set; } = false;
    
    public User User { get; set; }
    public User Recipient { get; set; }

}
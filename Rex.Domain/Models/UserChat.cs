namespace Rex.Models;

public sealed class UserChat: AuditableEntity
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    public Chat Chat { get; set; }

}
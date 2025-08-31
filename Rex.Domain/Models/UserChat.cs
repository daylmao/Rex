namespace Rex.Models;

public class UserChat: AuditableEntity
{
    public Guid? ChatId { get; set; }
    public Guid? UserId { get; set; }
}
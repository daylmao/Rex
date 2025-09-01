namespace Rex.Models;

public sealed class Message: AuditableEntity
{
    public string Description { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public Chat Chat { get; set; }
    public User Sender { get; set; }

}
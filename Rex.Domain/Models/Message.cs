namespace Rex.Models;

public class Message
{
    public string Description { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
}
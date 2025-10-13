
namespace Rex.Models;

public sealed class Chat: AuditableEntity
{
    public string Type { get; set; }
    public string? Name { get; set; }
    public string? GroupPhoto { get; set; } 
    public ICollection<UserChat> UserChats { get; set; }
    public ICollection<Message> Messages { get; set; }
}
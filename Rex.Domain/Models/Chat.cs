using Rex.Enum;

namespace Rex.Models;

public sealed class Chat: AuditableEntity
{
    public string Type { get; set; }
    
    public ICollection<UserChat> UserChats { get; set; }
    public ICollection<Message> Messages { get; set; }
}
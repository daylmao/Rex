using Rex.Enum;

namespace Rex.Models;

public class Chat: AuditableEntity
{
    public ChatType Type { get; set; }
}
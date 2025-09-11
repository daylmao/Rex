using Rex.Enum;

namespace Rex.Models;

public sealed class FriendShip: AuditableEntity
{
    public Guid TargetUserId { get; set; }
    public Guid RequesterId { get; set; }
    public string Status { get; set; }
    
    public User Requester { get; set; }
    public User TargetUser { get; set; }
}
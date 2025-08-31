using Rex.Enum;

namespace Rex.Models;

public class FriendShip: AuditableEntity
{
    public Guid AddressesId { get; set; }
    public Guid RequesterId { get; set; }
    public FriendStatus Status { get; set; }
}
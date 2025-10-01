
namespace Rex.Models;

public sealed class UserRole: AuditableEntity
{
    public string Role { get; set; }
    public ICollection<User> Users { get; set; }
}
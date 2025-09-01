
namespace Rex.Models;

public sealed class UserRole: EntityBase
{
    public string Role { get; set; }
    public ICollection<User> Users { get; set; }
}
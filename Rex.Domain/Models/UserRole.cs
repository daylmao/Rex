using Rex.Enum;

namespace Rex.Models;

public class UserRole: EntityBase
{
    public Enum.UserRole Role { get; set; }
}
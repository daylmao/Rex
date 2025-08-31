using Rex.Enum;

namespace Rex.Models;

public class Group: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public GroupVisibility Visibility { get; set; }
}
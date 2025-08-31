namespace Rex.Models;

public class UserGroup: AuditableEntity
{
    public Guid? UserId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? GroupRoleId { get; set; }
}
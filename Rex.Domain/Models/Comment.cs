namespace Rex.Models;

public class Comment: AuditableEntity
{
    public string Description { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public bool IsPinned { get; set; } = false;
    public Guid? ParentCommentId { get; set; }
    public bool Edited { get; set; } = false;
}
namespace Rex.Models;

public sealed class Comment: AuditableEntity
{
    public string Description { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public bool IsPinned { get; set; } = false;
    public Guid? ParentCommentId { get; set; }
    public bool Edited { get; set; } = false;
    
    public ICollection<Comment>? Replies { get; set; }
    public User User { get; set; }
    public Comment ParentComment { get; set; }
    public Post Post { get; set; }

}
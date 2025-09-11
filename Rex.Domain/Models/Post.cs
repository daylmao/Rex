namespace Rex.Models;

public sealed class Post: AuditableEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? ChallengeId { get; set; }
    
    public User User { get; set; }
    public Group Group { get; set; }
    public Challenge Challenge { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}
namespace Rex.Models;

public sealed class Challenge: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid CreatorId { get; set; }
    public Guid GroupId { get; set; }
    public TimeSpan Duration { get; set; }
    
    public ICollection<Post> Posts { get; set; }
    public Group Group { get; set; }
    public User Creator { get; set; }

}
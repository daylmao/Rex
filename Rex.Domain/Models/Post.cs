namespace Rex.Models;

public class Posts: AuditableEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public Guid? ChallengeId { get; set; }
}
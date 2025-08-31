namespace Rex.Models;

public class Challenge: AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid CreatorId { get; set; }
    public Guid GroupId { get; set; }
    public TimeSpan Duration { get; set; }
}

namespace Rex.Models;

public sealed class UserChallenge: AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid ChallengeId { get; set; }
    public string Status { get; set; }
    
    public User User { get; set; }
    public Challenge Challenge { get; set; }

}
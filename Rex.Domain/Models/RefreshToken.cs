namespace Rex.Models;

public sealed class RefreshToken: AuditableEntity
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public bool Used { get; set; } = false;
    public DateTime Expiration { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool Revoked { get; set; } = false;
    
    public User User { get; set; }
}
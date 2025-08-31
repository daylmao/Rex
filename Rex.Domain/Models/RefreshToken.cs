namespace Rex.Models;

public class RefreshToken: EntityBase
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public bool Used { get; set; } = false;
    public DateTime Expiration { get; set; }
    public DateTime Created { get; set; }
    public bool Revoked { get; set; } = false;
}
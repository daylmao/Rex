using Rex.Enum;

namespace Rex.Models;

public sealed class Code: AuditableEntity
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public DateTime Expiration { get; set; }
    public string Type { get; set; }
    public bool Revoked { get; set; } = false;
    public bool RefreshCode { get; set; } = false;
    public bool Used { get; set; } = false;
    
    public User User { get; set; }
}
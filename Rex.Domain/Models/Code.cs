using Rex.Enum;

namespace Rex.Models;

public class Code: AuditableEntity
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public DateTime Expiration { get; set; }
    public CodeType Type { get; set; }
    public bool Revoked { get; set; } = false;
    public bool RefreshCode { get; set; } = false;
}
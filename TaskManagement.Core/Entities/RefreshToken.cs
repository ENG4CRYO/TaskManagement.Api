
using System.ComponentModel.DataAnnotations;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public bool IsActive => RevokedOn == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
}


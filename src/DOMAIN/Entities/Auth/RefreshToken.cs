using System.ComponentModel.DataAnnotations;
using DOMAIN.Entities.Base;

namespace DOMAIN.Entities.Auth;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; init; }
    [StringLength(100)] public string Token { get; set; }
    public DateTime Expiry { get; init; }
}
using System.ComponentModel.DataAnnotations;
using DOMAIN.Entities.Base;
using DOMAIN.Entities.Users;

namespace DOMAIN.Entities.Auth;

public class PasswordReset : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    [StringLength(255)] public string Token { get; set; }
    [StringLength(255)] public string KeyName { get; set; }
}
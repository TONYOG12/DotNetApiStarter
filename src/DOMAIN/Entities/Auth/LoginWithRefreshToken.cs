using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Auth;

public class LoginWithRefreshToken
{
    [Required] public string ClientId { get; set; }
    [Required] public string RefreshToken { get; set; }
}
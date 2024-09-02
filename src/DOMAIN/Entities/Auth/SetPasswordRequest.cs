using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Auth;

public class SetPasswordRequest
{
    [Required] [MinLength(8)] public string Password { get; set; }
    [Required] public string ConfirmPassword { get; set; }
    [Required] public string Token { get; set; }
}
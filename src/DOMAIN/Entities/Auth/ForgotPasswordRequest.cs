using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Auth;

public class ForgotPasswordRequest 
{
    public string ClientId { get; set; }
    [EmailAddress] public string Email { get; set; }
}
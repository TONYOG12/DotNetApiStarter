using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Users;

public class CreateClientRequest
{
    [Required] [EmailAddress] public string Email { get; set;}
    [Required] public string Password { get; set;}
}
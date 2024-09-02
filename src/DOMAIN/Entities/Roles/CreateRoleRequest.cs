using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Roles;

public class CreateRoleRequest
{
    [Required] public string Name { get; set; }
    public List<string> Permissions { get; set; } = [];
}
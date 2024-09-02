using System.ComponentModel.DataAnnotations;

namespace DOMAIN.Entities.Roles;

public class UpdateRoleRequest
{
    [Required] public string Name { get; set; }
    [Required] public string DisplayName { get; set; }
}
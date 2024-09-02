using SHARED;

namespace DOMAIN.Entities.Roles;

public static class RoleErrors
{
    public static Error NotFound(Guid roleId) =>
        Error.NotFound("Roles.NotFound", $"The role with the Id: {roleId} was not found");
    public static Error InvalidRoleName(string roleName) => 
        Error.Validation("Roles.InvalidName", $"The role with name: {roleName} already exists.");
}
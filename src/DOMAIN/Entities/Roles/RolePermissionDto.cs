using SHARED;

namespace DOMAIN.Entities.Roles;

public class RolePermissionDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Name { get; set; }
    public List<PermissionResponseDto> Permissions { get; set; }
    public List<CollectionItemDto> Users { get; set; }
}
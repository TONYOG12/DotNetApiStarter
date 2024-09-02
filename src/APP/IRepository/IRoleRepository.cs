using APP.Utils;
using DOMAIN.Entities.Roles;
using SHARED;

namespace APP.IRepository;

public interface IRoleRepository
{
    Task<Result<List<RoleDto>>> GetRoles();
    Task<Result<Paginateable<IEnumerable<RolePermissionDto>>>> GetRolesWithPermissionsAndAssignees(int page, int pageSize, 
        string searchQuery);
    Task<Result<RolePermissionDto>> GetRole(Guid id);
    Task<Result> CreateRole(CreateRoleRequest request, Guid userId);
    Task<Result> UpdateRole(UpdateRoleRequest request, Guid id, Guid userid);
    Task<Result> DeleteRole(Guid id, Guid userId);
    Task<Result<dynamic>> CheckRole(Guid id);
}
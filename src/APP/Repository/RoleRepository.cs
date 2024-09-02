using System.Security.Claims;
using APP.Extensions;
using APP.IRepository;
using APP.Utils;
using AutoMapper;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SHARED;

namespace APP.Repository;

public class RoleRepository(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager /*, IPermissionRepository permissionRepository*/)
    : IRoleRepository
{
    public async Task<Result<List<RoleDto>>> GetRoles()
    {
        var roles =  await context.Roles.ToListAsync();
        return roles.Select(mapper.Map<RoleDto>).ToList();
    }

    public async Task<Result<Paginateable<IEnumerable<RolePermissionDto>>>> GetRolesWithPermissionsAndAssignees(int page, int pageSize, 
        string searchQuery)
    {
        var result = await GetRoles();
        var roles = mapper.Map<List<RolePermissionDto>>(result.Value);
        foreach (var role in roles)
        {
            //role.Permissions = await permissionRepository.GetPermissionByRole(role.Id);
            role.Users = mapper.Map<List<CollectionItemDto>>(await userManager.GetUsersInRoleAsync(role.Name));
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            roles = roles.Where(
                r => r.Name != null && r.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                     ||
                     r.DisplayName != null && r.DisplayName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                     ||
                     r.Users.Any(u => u.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                     ||
                     r.Permissions.Any(p
                         => p.Description != null && p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                         ||
                         p.GroupName != null && p.GroupName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                         ||
                         p.Action != null && p.Action.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                         )
            ).ToList();
        }
        
        return await PaginationHelper.GetPaginatedResultAsync(
            roles.AsQueryable(), 
            page, 
            pageSize, 
            mapper.Map<RolePermissionDto>
        );
    }

    public async Task<Result<RolePermissionDto>> GetRole(Guid id)
    {
        var role = mapper.Map<RolePermissionDto>(await context.Roles.FirstOrDefaultAsync(item => item.Id == id));
        //role.Permissions = await permissionRepository.GetPermissionByRole(role.Id);
        return role;
    }

    public async Task<Result> CreateRole(CreateRoleRequest request, Guid userId)
    {
        var newRole = new Role
        {
            Name = request.Name,
            DisplayName = request.Name.Capitalize(),
            CreatedById = userId
        };
        
        if (!await ValidRoleName(request.Name))
            return RoleErrors.InvalidRoleName(request.Name);
        
        var result = await roleManager.CreateAsync(newRole);
            
        if (!result.Succeeded)
        {
            return Error.Failure("Role.Create", $"{result.Errors.First()}");
        }
            
        foreach (var permission in request.Permissions)
        {
            await roleManager.AddClaimAsync(newRole, new Claim(AppConstants.Permission, permission));
        }

        return Result.Success();
    }

    public async Task<Result> UpdateRole(UpdateRoleRequest request, Guid id, Guid userid)
    {
        var role =await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return RoleErrors.NotFound(id);

        role.Name = request.Name;
        role.DisplayName = request.DisplayName;
        role.NormalizedName = request.Name.ToUpper();
        role.LastUpdatedById = userid;
        context.Roles.Update(role);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<dynamic>> CheckRole(Guid id)
    {
        var role = await context.Roles.FirstOrDefaultAsync(item => item.Id == id);
        if (role == null) RoleErrors.NotFound(id);
        
        var usersWithRole = await userManager.GetUsersInRoleAsync(role.Name);
        return new { HasUsers = usersWithRole.Count != 0 };
    }
    
    public async Task<Result> DeleteRole(Guid id, Guid userId)
    {
        var role = await context.Roles.FirstOrDefaultAsync(item => item.Id == id);
        if (role == null) return RoleErrors.NotFound(id);

        role.DeletedAt = DateTime.Now;
        role.LastUpdatedById = userId;
        context.Roles.Update(role);
        await context.SaveChangesAsync();

        return Result.Success();
    }
    
    private async Task<bool> ValidRoleName(string roleName)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        return role != null;
    }
}
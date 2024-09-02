using APP.Extensions;
using APP.IRepository;
using APP.Utils;
using DOMAIN.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages roles within the application.
/// </summary>
[Route("api/v{version:apiVersion}/role")]
[ApiController]
public class RoleController(IRoleRepository repo) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all roles.
    /// </summary>
    /// <returns>A list of roles.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleDto>))]
    public async Task<IResult> GetRoles()
    {
        var response = await repo.GetRoles();
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Retrieves a paginated list of roles along with their permissions and assignees.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchQuery">Optional search query to filter roles.</param>
    /// <returns>A paginated list of roles with permissions and assignees.</returns>
    [HttpGet("with-permissions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginateable<IEnumerable<RolePermissionDto>>))]
    public async Task<IResult> GetRolesWithPermissions([FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "pageSize")] int pageSize = 5,
        [FromQuery(Name = "searchQuery")] string searchQuery = null)
    {
        var response = await repo.GetRolesWithPermissionsAndAssignees(page, pageSize, searchQuery);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }

    /// <summary>
    /// Retrieves details of a specific role by ID.
    /// </summary>
    /// <param name="id">The ID of the role to retrieve.</param>
    /// <returns>The details of the role.</returns>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RolePermissionDto))]
    public async Task<IResult> GetRole(Guid id)
    {
        try
        {  
            var response = await repo.GetRole(id);
            return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
        }
        catch (Exception e)
        {
            return TypedResults.NotFound(e.Message);
        }
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="request">The request containing role details.</param>
    /// <returns>An empty response indicating success.</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> CreateRole(CreateRoleRequest request)
    {
        var userId = (string)HttpContext.Items["Sub"];
        if (userId == null) return TypedResults.Unauthorized();
        
        var response = await repo.CreateRole(request, Guid.Parse(userId));
        return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
    }

    /// <summary>
    /// Updates an existing role by ID.
    /// </summary>
    /// <param name="request">The request containing updated role details.</param>
    /// <param name="id">The ID of the role to update.</param>
    /// <returns>An empty response indicating success.</returns>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> UpdateRole(UpdateRoleRequest request, Guid id)
    {
        try
        {
            var userId = (string)HttpContext.Items["Sub"];
            if (userId == null) return TypedResults.Unauthorized();
        
            var response = await repo.UpdateRole(request, id, Guid.Parse(userId));
            return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
        }
        catch (Exception e)
        {
            return TypedResults.NotFound(e.Message);
        }
    }

    /// <summary>
    /// Checks if a specific role exists by ID.
    /// </summary>
    /// <param name="id">The ID of the role to check.</param>
    /// <returns>A result indicating whether the role exists.</returns>
    [Authorize]
    [HttpGet("check/{id}")]
    public async Task<IResult> CheckRole(Guid id)
    {
        var response = await repo.CheckRole(id);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }

    /// <summary>
    /// Deletes an existing role by ID.
    /// </summary>
    /// <param name="id">The ID of the role to delete.</param>
    /// <returns>An empty response indicating success.</returns>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IResult> DeleteRole(Guid id)
    {
        try
        {  
            var userId = (string)HttpContext.Items["Sub"];
            if (userId == null) return TypedResults.Unauthorized();
            
            var response = await repo.DeleteRole(id, Guid.Parse(userId));
            return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
        }
        catch (Exception e)
        {
            return TypedResults.NotFound(e.Message);
        }
    }
}

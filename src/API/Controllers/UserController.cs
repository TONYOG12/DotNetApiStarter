using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APP.Extensions;
using APP.IRepository;
using APP.Utils;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using SHARED.Requests;

namespace API.Controllers;

/// <summary>
/// Controller for managing users.
/// </summary>
[Route("api/v{version:apiVersion}/user")]
[ApiController]
public class UserController(IUserRepository repo) : ControllerBase
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The request object containing user details.</param>
    /// <returns>A result indicating the success or failure of the user creation.</returns>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = await repo.CreateUser(request);
        return response.IsSuccess ? TypedResults.Ok(response) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Signs up a new client.
    /// </summary>
    /// <param name="request">The request object containing client details.</param>
    /// <returns>The created user information.</returns>
    [AllowAnonymous]
    [HttpPost("sign-up")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IResult> CreateNewUser([FromBody] CreateClientRequest request)
    {
        var response = await repo.CreateNewUser(request);
        return response.IsSuccess ? TypedResults.Created("", response.Value) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of users per page.</param>
    /// <param name="roleNames">Filter users by roles.</param>
    /// <param name="searchQuery">Search query for user information.</param>
    /// <param name="withDisabled">Include disabled users in the results.</param>
    /// <returns>A paginated list of users.</returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paginateable<IEnumerable<UserDto>>))]
    public async Task<IResult> GetUsers([FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "pageSize")] int pageSize = 5,
        [FromQuery(Name = "roleNames")] string roleNames = null,
        [FromQuery(Name = "searchQuery")] string searchQuery = null,
        [FromQuery(Name = "with-disabled")] bool withDisabled = false)
    {
        var response = await repo.GetUsers(page, pageSize, searchQuery, roleNames, withDisabled);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="request">The update request object containing user details.</param>
    /// <param name="id">The ID of the user to update.</param>
    /// <returns>No content if the update is successful.</returns>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateUser([FromBody] UpdateUserRequest request, Guid id)
    {
        var userId = (string)HttpContext.Items["Sub"];
        if (userId == null) return TypedResults.Unauthorized();
        
        var response = await repo.UpdateUser(request, id, Guid.Parse(userId));
        return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Updates the roles of an existing user.
    /// </summary>
    /// <param name="request">The update request object containing role names.</param>
    /// <param name="id">The ID of the user to update roles for.</param>
    /// <returns>No content if the role update is successful.</returns>
    [HttpPut("role/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateUserRoles([FromBody] UpdateUserRoleRequest request, Guid id)
    {
        var userId = (string)HttpContext.Items["Sub"];
        if (userId == null) return TypedResults.Unauthorized();

        var response = await repo.UpdateRolesOfUser(request, id, Guid.Parse(userId));
        return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
    }

    /// <summary>
    /// Deletes an existing user.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content if the deletion is successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteUser(Guid id)
    {
        try
        {  
            var userId = (string)HttpContext.Items["Sub"];
            if (userId == null) return TypedResults.Unauthorized();
            
            var response = await repo.DeleteUser(id, Guid.Parse(userId));
            return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
        }
        catch (Exception e)
        {
            return TypedResults.NotFound(e.Message);
        }
    }
    
    /// <summary>
    /// Uploads an avatar image for a user.
    /// </summary>
    /// <param name="request">The file upload request containing the image data.</param>
    /// <param name="id">The ID of the user to upload the image for. If null, uses the current user's ID.</param>
    /// <returns>No content if the upload is successful.</returns>
    [AllowAnonymous]
    [HttpPost("avatar/{id?}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> UploadAnImage([Required][FromBody] UploadFileRequest request, Guid? id = null)
    {
        try
        {
            var userId = (string)HttpContext.Items["Sub"];
            if (userId == null) return TypedResults.Unauthorized();
        
            await repo.UploadAvatar(request, id ?? Guid.Parse(userId));
            return  TypedResults.NoContent();
        }
        catch (Exception)
        {
            return TypedResults.NoContent();
        }
    }
    
    /// <summary>
    /// Toggles the disable state of an existing user.
    /// </summary>
    /// <param name="id">The ID of the user to toggle the disable state for.</param>
    /// <returns>No content if the toggle is successful.</returns>
    [HttpGet("toggle-disable/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> DisableUser(Guid id)
    {
        try
        {  
            var userId = (string)HttpContext.Items["Sub"];
            if (userId == null) return TypedResults.Unauthorized();
            
            var response = await repo.ToggleDisableUser(id, Guid.Parse(userId));
            return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
        }
        catch (Exception e)
        {
            return TypedResults.NotFound(e.Message);
        }
    }
}

using APP.IRepository;
using APP.Services.Storage;
using APP.Services.Token;
using APP.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using APP.Extensions;
using DOMAIN.Entities.Auth;
using DOMAIN.Entities.Roles;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;
using SHARED;
using SHARED.Requests;

namespace APP.Repository;

public class UserRepository(ApplicationDbContext context, UserManager<User> userManager, IJwtService jwtService, IBlobStorageService blobStorage, IMapper mapper)
    : IUserRepository
{
    public async Task<Result<Guid>> CreateUser(CreateUserRequest request)
    {
        var user = mapper.Map<User>(request);
        user.UserName = request.Email;

        if (!await EmailIsUnique(request.Email))
            return UserErrors.EmailNotUnique;

        var result = await userManager.CreateAsync(user, request.Password);
            
        List<Error> errors = [];
        
        if (!result.Succeeded)
        {
            errors.AddRange(result.Errors.Select(error => Error.Validation(error.Code, error.Description)));
        }

        if (errors.Count != 0) return errors;
        
        var roleName = context.Roles
            .Where(item => request.RoleNames.Contains(item.Name))
            .Select(item => item.Name)
            .ToList();
            
        await userManager.AddToRolesAsync(user, roleName);
        await context.SaveChangesAsync();
        
        if (string.IsNullOrEmpty(request.Avatar))  return user.Id;
        
        if (!request.Avatar.IsValidBase64String()) return UserErrors.InvalidAvatar;
         
        var image = request.Avatar.ConvertFromBase64();
        var reference = $"{user.Id}.{image.FileName.Split(".").Last()}";
        var uploadResult = await blobStorage.UploadBlobAsync("avatar", image, reference);
        if (uploadResult.IsSuccess)
        {
            user.Avatar = reference;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        
        return user.Id;
    }

    public async Task<Result<LoginResponse>> CreateNewUser(CreateClientRequest request)
    {
        var user = mapper.Map<User>(request);
        user.UserName = request.Email;

        if (!await EmailIsUnique(request.Email))
            return UserErrors.EmailNotUnique;

        var result = await userManager.CreateAsync(user, request.Password);
            
        if (!result.Succeeded)
        {
            return Result.Failure<LoginResponse>(Error.Failure("User.Create", result.Errors.First().Description));
        }
            
        var roleName = context.Roles
            .Where(item => item.Name == RoleUtils.AppRoleSuper)
            .Select(item => item.Name)
            .ToList();

        await userManager.AddToRolesAsync(user, roleName);

        await context.SaveChangesAsync();

        return await jwtService.AuthenticateNewUser(user);
    }

    public async Task<Result<Paginateable<IEnumerable<UserDto>>>> GetUsers(int page, int pageSize, string searchQuery, string roleNames, bool withDisabled = false)
    {
        var query = context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(roleNames))
        {
            var roles = roleNames.Split(",").Select(str => str.Trim()).ToList();
            var usersInRoles = new List<User>();

            foreach (var role in roles)
            {
                var roleUsers = await userManager.GetUsersInRoleAsync(role);
                usersInRoles.AddRange(roleUsers);
            }
            
            query = usersInRoles.Distinct().AsQueryable();
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.WhereSearch(searchQuery, q => q.FirstName, q => q.LastName, q => q.Email);
        }

        return await PaginationHelper.GetPaginatedResultAsync(
            query, 
            page, 
            pageSize, 
            mapper.Map<UserDto>
        );
    }
    
    public async Task<Result<UserDto>> GetUser(Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return UserErrors.NotFound(userId);
        return mapper.Map<UserDto>(user);
    }

    public async Task<Result> UpdateUser(UpdateUserRequest request, Guid id, Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id);
        if (user != null)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.HiredOn = request.HiredOn;
            user.DateOfBirth = request.DateOfBirth;
            user.DirectReportId= request.DirectReportId;
            user.HiredOn =request.HiredOn;
            user.LastUpdatedById = userId;
            user.UpdatedAt = DateTime.UtcNow;

            context.Users.Update(user);
            await context.SaveChangesAsync();
            
            if (!string.IsNullOrEmpty(request.Avatar))
            {
                var image = request.Avatar.ConvertFromBase64();
                var reference = $"{user.Id}.{image.FileName.Split(".").Last()}";
                await blobStorage.UploadBlobAsync("avatar", image, reference, user.Avatar);
             
                user.Avatar = reference;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.UserName = request.Email;
                user.Email = request.Email;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                await userManager.UpdateNormalizedEmailAsync(user);
                await userManager.UpdateNormalizedUserNameAsync(user);
            }
        }

        return Result.Success();
    }
    
    public async Task<Result> UpdateRolesOfUser(UpdateUserRoleRequest request, Guid id, Guid userId)
    {
        foreach (var roleName in request.RoleNames)
        {
            if (!await ValidRoleName(roleName))
            {
                return UserErrors.InvalidRoleName(roleName);
            }
        }
        
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id);
        var existingRoles =  await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, existingRoles);
        await userManager.AddToRolesAsync(user, request.RoleNames);
        user.LastDeletedById = userId;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteUser(Guid id, Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return UserErrors.NotFound(userId);

        user.DeletedAt = DateTime.UtcNow;
        user.LastDeletedById = userId;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        
        return Result.Success();
    }
    
    public async Task<Result> ToggleDisableUser(Guid id, Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id);
        if (user == null) return UserErrors.NotFound(userId);
        
        user.IsDisabled = !user.IsDisabled;
        user.LastDeletedById = userId;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UploadAvatar(UploadFileRequest request, Guid userId)
    {
        var avatar = request.File.ConvertFromBase64();
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return UserErrors.NotFound(userId);
        var reference = $"{userId}.{avatar.FileName.Split(".").Last()}";

        var result = await blobStorage.UploadBlobAsync("avatar", avatar, reference, user.Avatar);
        if (result.IsSuccess)
        {
            user.Avatar = reference;
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        return result;
    }
    
    private async Task<bool> EmailIsUnique(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user == null;
    }

    private async Task<bool> ValidRoleName(string roleName)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        return role != null;
    }
}
using SHARED;

namespace DOMAIN.Entities.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with the Id: {userId} was not found");
    
    public static Error NotFoundByEmail(string email) =>
        Error.NotFound("Users.NotFound", $"The user with the email: {email} was not found");
    
    public static Error EmailNotUnique =>
        Error.Validation("Users.EmailNotUnique", $"The email provided already exists in the system");
    
    public static Error InvalidAvatar =>
        Error.Validation("Users.Avatar", "Invalid avatar");
    public static Error IncorrectCredentials =>
        Error.Validation("Users.Details", $"Your credentials do not match our records");
    
    public static Error InvalidRoleName(string roleName) =>
        Error.Validation("Users.Roles", $"Role name: {roleName} is not valid.");
}
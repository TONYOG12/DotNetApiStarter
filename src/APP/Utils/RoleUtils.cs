namespace APP.Utils;

public class RoleUtils
{
    public const string AppRoleSuper = "super";
    public const string AppRoleAdmin = "admin";
 
    
    public static IEnumerable<string> AppRoles()
    {
        return new[]
        {
            AppRoleSuper,
            AppRoleAdmin
        };
    }
}
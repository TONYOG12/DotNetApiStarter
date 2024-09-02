using SHARED;

namespace DOMAIN.Entities.Auth;

public static class AuthErrors
{
    public static Error NotFound =>
        Error.NotFound("Token.NotFound", $"The reset token was not found");
    
    public static Error TokenExpired =>
        Error.NotFound("Token.Expired", $"The reset token has expired. Kindly request a new one");
}
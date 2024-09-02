using DOMAIN.Entities.Auth;
using DOMAIN.Entities.Users;
using SHARED;

namespace APP.Services.Token;

public interface IJwtService
{
    Task<Result<LoginResponse>> Authenticate(User user, string clientId);
    Task<Result<LoginResponse>> AuthenticateById(Guid? id, string clientId);
    Task<Result<LoginResponse>> AuthenticateNewUser(User user);
}
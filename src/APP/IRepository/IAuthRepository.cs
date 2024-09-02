using Microsoft.AspNetCore.Identity.Data;
using DOMAIN.Entities.Auth;
using SHARED;
using ForgotPasswordRequest = DOMAIN.Entities.Auth.ForgotPasswordRequest;

namespace APP.IRepository;

public interface IAuthRepository
{ 
    Task<Result<LoginResponse>> Login(LoginRequest request);
    Task<Result<LoginResponse>> LoginWithRefreshToken(LoginWithRefreshToken request);
    Task<Result> ForgotPassword(ForgotPasswordRequest request);
    Task<Result<PasswordChangeResponse>> SetPassword(SetPasswordRequest model);
    Task<Result<PasswordChangeResponse>> ResetPassword(ResetPasswordRequest model, Guid userId);
}
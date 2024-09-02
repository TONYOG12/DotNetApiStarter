using APP.IRepository;
using APP.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using APP.Extensions;
using DOMAIN.Entities.Auth;
using DOMAIN.Entities.Users;
using INFRASTRUCTURE.Context;
using SHARED;
using ForgotPasswordRequest = DOMAIN.Entities.Auth.ForgotPasswordRequest;

namespace APP.Repository;

public class AuthRepository(ApplicationDbContext context, UserManager<User> userManager, IJwtService jwtService /*, IEmailService emailService*/) 
    : IAuthRepository
{
    public async Task<Result<LoginResponse>> Login(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Email == request.Email);

        if (user == null || user.IsDeleted())
        {
            return UserErrors.NotFoundByEmail(request.Email);
        }
        
        if (user.PasswordHash == null) return UserErrors.IncorrectCredentials;
        var verifyRes = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        switch (verifyRes)
        {
            case PasswordVerificationResult.SuccessRehashNeeded:
            {
                var hashPassword = userManager.PasswordHasher.HashPassword(user, request.Password);
                user.PasswordHash = hashPassword;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                break;
            }
            case PasswordVerificationResult.Failed:
                return UserErrors.IncorrectCredentials;
        }
        
        try
        {
            return await jwtService.Authenticate(user, "web");
        }
        catch (Exception)
        {
            return Error.Failure("Login.Error", "Unable to login user");
        }
    }

    public async Task<Result<LoginResponse>> LoginWithRefreshToken(LoginWithRefreshToken request)
    {
        var details = await context.RefreshTokens.FirstOrDefaultAsync(item => item.Token == request.RefreshToken);

        if (details == null)
            return Error.NotFound("Token.NotFound",
                $"Refresh token {request.RefreshToken} was not found in the system");

        return await jwtService.AuthenticateById(details.UserId, "web");  
    }

    public async Task<Result> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return UserErrors.NotFoundByEmail(request.Email);
        }
        
        var partialUrl = Environment.GetEnvironmentVariable("clientBaseUrl");
        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var key = Guid.NewGuid().ToString();

        context.PasswordResets.Add(new PasswordReset
        {
            UserId = user.Id,
            Token = token,
            KeyName = key,
            CreatedAt = DateTime.Now
        });

        await context.SaveChangesAsync();

        var url = request.ClientId switch
        {
            "web" => $"https://{user.OrganizationName.ToLower()}.{partialUrl}/reset-password?key={key}",
            "mobile" => $"veilghehs-mobile://set-password/{key}",
            _ => $"{partialUrl}/reset-password?key={key}"
        };

        //await emailService.SendForgotPasswordEmail(url, user);
        return Result.Success();
    }
    
    public async Task<Result<PasswordChangeResponse>> SetPassword(SetPasswordRequest model)
    {
        var tokenDetails = await context.PasswordResets.FirstOrDefaultAsync(c => c.KeyName == model.Token);

        if (tokenDetails == null)
        {
            return AuthErrors.NotFound;
        }
            
        if (tokenDetails.CreatedAt.AddDays(1) < DateTime.Now)
        {
            return AuthErrors.TokenExpired;
        }

        var user = await userManager.FindByIdAsync(tokenDetails.UserId.ToString());

        if (user == null) return UserErrors.NotFound(tokenDetails.UserId);
            
        var result = await userManager.ResetPasswordAsync(user, tokenDetails.Token, model.Password);
            
        if (!result.Succeeded)
            return new PasswordChangeResponse
            {
                Success = false,
                Errors = result.Errors.Select(item => item.Description)
            };
        user.EmailConfirmed = true;
        return new PasswordChangeResponse
        {
            Success = true
        };
    }

    public async Task<Result<PasswordChangeResponse>> ResetPassword(ResetPasswordRequest model, Guid userId)
    {
        var user = await context.Users.SingleOrDefaultAsync(item => item.Id == userId);

        if (user == null) return UserErrors.NotFound(userId);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
        if (result.Succeeded)
        {
            return new PasswordChangeResponse
            {
                Success = true
            };
        }

        return new PasswordChangeResponse
        {
            Success = false,
            Errors = result.Errors.Select(item => item.Description)
        };
    }
}
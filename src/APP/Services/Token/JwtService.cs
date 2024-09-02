using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using INFRASTRUCTURE.Context;
using DOMAIN.Entities.Auth;
using DOMAIN.Entities.Users;
using SHARED;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace APP.Services.Token;

public class JwtService(ApplicationDbContext context, IConfiguration configuration, UserManager<User> userManager)
    : IJwtService
{
    public async Task<Result<LoginResponse>> Authenticate(User user, string clientId)
    {
        var tokenJson = await GenerateToken(user, clientId);
        if (tokenJson.IsFailure)
            return tokenJson.Error;

        var refreshToken = SaveRefreshToken(user.Id);
        var expiry = DateTime.UtcNow.AddMonths(6);
        
        return new LoginResponse
        {
            UserId = user.Id,
            AccessToken = tokenJson.Value,
            RefreshToken = refreshToken,
            ExpiresIn = Convert.ToInt32(expiry.Subtract(DateTime.UtcNow).TotalSeconds)
        };
    }
    
    private static string GenerateRefreshToken(int size = 32) 
    { 
        var randomNumber = new byte[size]; 
        using var rng = RandomNumberGenerator.Create(); 
        rng.GetBytes(randomNumber); 
        return Convert.ToBase64String(randomNumber);
    }

    private string SaveRefreshToken(Guid userId) 
    { 
        var refreshToken = GenerateRefreshToken(); 
        context.RefreshTokens.Add(new RefreshToken 
        { 
            CreatedById = userId, 
            Token = refreshToken, 
            Expiry = DateTime.UtcNow.AddMonths(6)
        });
        context.SaveChanges();
        return refreshToken;
    }
    
    private async Task<Result<string>> GenerateToken(User user, string clientId)
    { 
        var jwtKey = configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            return Error.NotFound("JwtKey.NotFound", "Jwt ket not found");
        
        var keyBytes = Encoding.ASCII.GetBytes(jwtKey);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var expiry = DateTime.UtcNow.AddMonths(6);
        
        var roles = await userManager.GetRolesAsync(user);
        
        var claims = new List<Claim> 
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}" ?? ""), 
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            
        }; 
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        { 
            Subject = new ClaimsIdentity(claims), 
            Expires = expiry,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
        };

        var tokenJson = tokenHandler.CreateEncodedJwt(tokenDescriptor); 
        return tokenJson;
    }

    public async Task<Result<LoginResponse>> AuthenticateById(Guid? id, string clientId) 
    { 
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id); 
        if (user != null)
        {
            return await Authenticate(user, clientId);
        }

        return Result.Failure<LoginResponse>(Error.NotFound("User.NotFound", "User with {id} not found"));
    }
        
    public async Task<Result<LoginResponse>> AuthenticateNewUser(User user) 
    { 
        var clientId = string.Empty; 
        var tokenJson =  await GenerateToken(user, clientId); 
        var refreshToken = SaveRefreshToken(user.Id); 
        var expiry = DateTime.UtcNow.AddMonths(6);
        
        return new LoginResponse 
        { 
            AccessToken = tokenJson.Value, 
            RefreshToken = refreshToken, 
            ExpiresIn = Convert.ToInt32(expiry.Subtract(DateTime.UtcNow).TotalSeconds)
        };
    }
}
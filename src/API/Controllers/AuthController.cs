using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using APP.Extensions;
using APP.IRepository;
using DOMAIN.Entities.Auth;
using ForgotPasswordRequest = DOMAIN.Entities.Auth.ForgotPasswordRequest;

namespace API.Controllers;

[Route("api/v{version:apiVersion}/auth")]
[ApiController]
public class AuthController(IAuthRepository repo) : ControllerBase
{
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request)
    {
        var response = await repo.Login(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [AllowAnonymous]
    [HttpPost("login-with-refresh-token")]
    public async Task<IResult> Login([FromBody] LoginWithRefreshToken request)
    {
        var response = await repo.LoginWithRefreshToken(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PasswordChangeResponse))]
    [AllowAnonymous]
    [HttpPost("set-password")]
    public async Task<IResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var response = await repo.SetPassword(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PasswordChangeResponse))]
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var userId = (string)HttpContext.Items["Sub"];

        var response = await repo.ResetPassword(request, Guid.Parse(userId));
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await repo.ForgotPassword(request);
        return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
    }
}
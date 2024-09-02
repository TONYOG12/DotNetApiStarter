using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APP.Extensions;
using APP.IRepository;
using DOMAIN.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;
using ForgotPasswordRequest = DOMAIN.Entities.Auth.ForgotPasswordRequest;

namespace API.Controllers;

/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
[Route("api/v{version:apiVersion}/auth")]
[ApiController]
public class AuthController(IAuthRepository repo) : ControllerBase
{
    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login request containing the username and password.</param>
    /// <returns>A result with the login response containing the JWT token and refresh token.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request)
    {
        var response = await repo.Login(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Logs in a user using a refresh token and returns a new JWT token.
    /// </summary>
    /// <param name="request">The request containing the refresh token.</param>
    /// <returns>A result with the login response containing the new JWT token and refresh token.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [AllowAnonymous]
    [HttpPost("login-with-refresh-token")]
    public async Task<IResult> Login([FromBody] LoginWithRefreshToken request)
    {
        var response = await repo.LoginWithRefreshToken(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }
    
    /// <summary>
    /// Sets a new password for a user who has requested a password reset.
    /// </summary>
    /// <param name="request">The request containing the new password, confirmation of the password, and the reset token.</param>
    /// <returns>A result indicating the success or failure of the password change.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PasswordChangeResponse))]
    [AllowAnonymous]
    [HttpPost("set-password")]
    public async Task<IResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var response = await repo.SetPassword(request);
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }


    /// <summary>
    /// Changes the password of the currently authenticated user.
    /// </summary>
    /// <param name="request">The request containing the current password and the new password.</param>
    /// <returns>A result indicating the success or failure of the password change.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PasswordChangeResponse))]
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var userId = (string)HttpContext.Items["Sub"];

        var response = await repo.ResetPassword(request, Guid.Parse(userId));
        return response.IsSuccess ? TypedResults.Ok(response.Value) : response.ToProblemDetails();
    }

    /// <summary>
    /// Initiates a password reset process for a user by sending a reset link to their email.
    /// </summary>
    /// <param name="request">The request containing the user's email.</param>
    /// <returns>A result indicating the success or failure of initiating the password reset process.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await repo.ForgotPassword(request);
        return response.IsSuccess ? TypedResults.NoContent() : response.ToProblemDetails();
    }
}

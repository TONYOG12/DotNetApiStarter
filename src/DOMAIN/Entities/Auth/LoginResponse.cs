namespace DOMAIN.Entities.Auth;

public class LoginResponse
{
    public Guid? UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public string Avatar { get; set; }
}
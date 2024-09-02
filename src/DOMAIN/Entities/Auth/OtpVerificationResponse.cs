namespace DOMAIN.Entities.Auth;

public class OtpVerificationResponse
{
    public bool Success { get; set; }
    public string Token { get; set; }
}
namespace DOMAIN.Entities.Auth;

public class PasswordChangeResponse
{
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
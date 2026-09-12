namespace Gadema.Core.Dtos.Authentication;

/// <summary>POST /api/v1/auth/google/signin — client sends the Google ID token it obtained.</summary>
public class GoogleSignInDto
{
    public string IdToken { get; set; } = "";
}
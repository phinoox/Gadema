// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Authentication;

/// <summary>
/// DTO for user registration with email/password authentication.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Email address (must be valid format, unique in system).
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(256)]
    public string Email { get; set; } = "";

    /// <summary>
    /// Password (min 8 chars, must contain uppercase, lowercase, number, special char).
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
                       ErrorMessage = "Password must contain uppercase letter, lowercase letter, number, and special character.")]
    public string Password { get; set; } = "";

    /// <summary>
    /// Confirm password (must match Password).
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? PasswordConfirmation { get; set; } = null!;

    /// <summary>
    /// Full name of the user.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(128)]
    public string Name { get; set; } = "";
}
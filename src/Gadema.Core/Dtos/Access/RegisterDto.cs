// =============================================================================
namespace Gadema.Core.Dtos.Access;

/// <summary>
/// Data transfer object for registering a new user with email and password authentication.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// The unique email address to be used for the account.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(256)]
    public string Email { get; set; } = "";

    /// <summary>
    /// The password for the new account, requiring complexity (uppercase, lowercase, number, and special character).
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$", 
                       ErrorMessage = "Password must contain uppercase letter, lowercase letter, number, and special character.")]
    public string Password { get; set; } = "";

    /// <summary>
    /// Confirmation of the password to ensure accuracy.
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? PasswordConfirmation { get; set; } = null!;

    /// <summary>
    /// The full name of the user for their profile.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(128)]
    public string Name { get; set; } = "";
}
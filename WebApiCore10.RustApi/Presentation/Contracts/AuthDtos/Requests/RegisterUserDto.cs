using System.ComponentModel.DataAnnotations;

namespace WebApiCore10.RustApi.Presentation.Contracts.AuthDtos.Requests
{
    /// <summary>
    /// Data required to register a new user.
    /// </summary>
    public sealed record RegisterUserDto
    {
        /// <summary>
        /// User's email address (used as login).
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// User's password (never stored in plain text).
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character."
        )]
        public string Password { get; init; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; init; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; init; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; init; }
    }
}

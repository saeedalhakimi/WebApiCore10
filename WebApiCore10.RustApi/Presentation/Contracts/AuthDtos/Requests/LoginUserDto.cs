using System.ComponentModel.DataAnnotations;

namespace WebApiCore10.RustApi.Presentation.Contracts.AuthDtos.Requests
{
    public sealed record LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}

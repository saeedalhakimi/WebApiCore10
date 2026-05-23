using System.ComponentModel.DataAnnotations;

namespace WebApiCore10.RustApi.Presentation.Contracts.AuthDtos.Requests
{
    public sealed record RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; init; } = string.Empty;
    }
}

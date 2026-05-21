namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Contracts.Responses
{
    public record ResponseWithTokensDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
    }
}

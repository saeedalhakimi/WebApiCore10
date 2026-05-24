namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands
{
    public sealed record LogoutCommand(
        string RefreshToken,
        string CorrelationId
    );
}

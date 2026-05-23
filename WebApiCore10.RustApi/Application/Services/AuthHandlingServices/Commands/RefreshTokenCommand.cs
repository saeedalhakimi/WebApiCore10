namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands
{
    public sealed record RefreshTokenCommand(
        string RefreshToken,
        string CorrelationId
    );
}

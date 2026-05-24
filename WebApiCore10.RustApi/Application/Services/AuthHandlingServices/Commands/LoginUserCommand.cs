namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands
{
    public sealed record LoginUserCommand(
        string Email,
        string Password,
        string CorrelationId
    );
}

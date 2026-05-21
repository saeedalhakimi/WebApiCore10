namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands
{
    /// <summary>
    /// Command to register a new user.
    /// Carries normalized and validated data.
    /// </summary>
    public sealed record RegisterUserCommand
    (
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string CorrelationId
    );
}

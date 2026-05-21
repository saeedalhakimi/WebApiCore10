using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands;
using WebApiCore10.RustApi.Presentation.Contracts.AuthDtos.Requests;

namespace WebApiCore10.RustApi.Presentation.Mappers.AuthMappers
{
    public static class AuthMapper
    {
        public static RegisterUserCommand ToRegisterUserCommand(RegisterUserDto dto, string correlationId)
        {
            return new RegisterUserCommand(
                Email: dto.Email.Trim().ToLowerInvariant(),
                Password: dto.Password,
                FirstName: dto.FirstName.Trim(),
                LastName: dto.LastName.Trim(),
                DateOfBirth: dto.DateOfBirth,
                CorrelationId: correlationId
            );
        }
    }
}

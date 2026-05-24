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

        public static LoginUserCommand ToLoginUserCommand(LoginUserDto dto, string correlationId)
        {
            return new LoginUserCommand(
                Email: dto.Email.Trim().ToLowerInvariant(),
                Password: dto.Password,
                CorrelationId: correlationId
            );
        }

        public static RefreshTokenCommand ToRefreshTokenCommand(RefreshTokenDto dto, string correlationId)
        {
            return new RefreshTokenCommand(
                RefreshToken: dto.RefreshToken,
                CorrelationId: correlationId
            );
        }

        public static LogoutCommand ToLogoutCommand(RefreshTokenDto dto, string correlationId)
        {
            return new LogoutCommand(
                RefreshToken: dto.RefreshToken,
                CorrelationId: correlationId
            );
        }
    }
}

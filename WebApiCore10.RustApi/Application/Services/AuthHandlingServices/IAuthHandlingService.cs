using SharedDll.Results;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Contracts.Responses;

namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices
{
    public interface IAuthHandlingService
    {
        Task<OperationResult<ResponseWithTokensDto>> RegisterUserCommandHandler(RegisterUserCommand command, CancellationToken cancellationToken);
        Task<OperationResult<ResponseWithTokensDto>> LoginUserCommandHandler(LoginUserCommand command, CancellationToken cancellationToken);
        Task<OperationResult<ResponseWithTokensDto>> RefreshTokenCommandHandler(RefreshTokenCommand command, CancellationToken cancellationToken);
        Task<OperationResult<bool>> LogoutCommandHandler(LogoutCommand command, CancellationToken cancellationToken);
    }
}

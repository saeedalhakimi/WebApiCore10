using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Contracts.Responses;
using WebApiCore10.RustApi.Presentation.Contracts.AuthDtos.Requests;
using WebApiCore10.RustApi.Presentation.Filters;
using WebApiCore10.RustApi.Presentation.Mappers.AuthMappers;
using WebApiCore10.RustApi.Presentation.Models;
using WebApiCore10.RustApi.Presentation.Routing;

namespace WebApiCore10.RustApi.Controllers.V1.AuthCtr
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.AuthRoutes.BaseRoute)]
    [ApiController]
    public class AuthController : BaseController<AuthController>
    {
        private readonly IAuthHandlingService _authHandlingService;

        public AuthController(ILogger<AuthController> logger, IAuthHandlingService authenticationService)
            : base(logger)
        {
            _authHandlingService = authenticationService;
        }

        [HttpPost(ApiRoutes.AuthRoutes.Register, Name = "Register")]
        [ProducesResponseType(typeof(ResponseWithTokensDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<ActionResult<ResponseWithTokensDto>> RegisterUser([FromBody] RegisterUserDto dto, CancellationToken cancellationToken)
        {
            using var _ = BeginRequestScope();

            var command = AuthMapper.ToRegisterUserCommand(dto, CorrelationId);
            var result = await _authHandlingService.RegisterUserCommandHandler(command, cancellationToken);
            if (!result.IsSuccess) return HandleResult(result);

            return Created(string.Empty, result.Data);
        }

        [HttpPost(ApiRoutes.AuthRoutes.RefreshToken, Name = "RefreshToken")]
        [ProducesResponseType(typeof(ResponseWithTokensDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<ActionResult<ResponseWithTokensDto>> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
        {
            using var _ = BeginRequestScope();

            var command = AuthMapper.ToRefreshTokenCommand(dto, CorrelationId);
            var result = await _authHandlingService.RefreshTokenCommandHandler(command, cancellationToken);
            if (!result.IsSuccess) return HandleResult(result);

            return Ok(result.Data);
        }
    }
}

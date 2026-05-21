using DoimanDlls.Exceptions;
using DoimanDlls.UserProfiles;
using DoimanDlls.UserProfiles.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedDll.Results;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Commands;
using WebApiCore10.RustApi.Application.Services.AuthHandlingServices.Contracts.Responses;
using WebApiCore10.RustApi.Application.Services.ErrorHandlingServices;
using WebApiCore10.RustApi.Application.Services.JWTServices;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models;

namespace WebApiCore10.RustApi.Application.Services.AuthHandlingServices
{
    public class AuthHandlingService : IAuthHandlingService
    {
        private const string DefaultRole = "User";

        private readonly ILogger<AuthHandlingService> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;

        public AuthHandlingService(
            ILogger<AuthHandlingService> logger,
            IErrorHandlingService errorHandlingService,
            DataContext dataContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService)
        {
            _logger = logger;
            _errorHandlingService = errorHandlingService;
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<OperationResult<ResponseWithTokensDto>>
            RegisterUserCommandHandler(
                RegisterUserCommand command,
                CancellationToken cancellationToken)
        {
            try
            {
                // --------------------------------------------------
                // 1️ Check if user already exists
                // --------------------------------------------------
                if (await _userManager.FindByEmailAsync(command.Email)
                    is not null)
                {
                    return _errorHandlingService
                        .HandleResourceConflictError<ResponseWithTokensDto>(
                            command.Email,
                            command.CorrelationId);
                }

                // --------------------------------------------------
                // 3️ EF Core execution strategy
                // --------------------------------------------------
                var strategy =
                    _dataContext.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async () =>
                {
                    // --------------------------------------------------
                    // 4️ Begin Transaction
                    // --------------------------------------------------
                    await using var transaction =
                        await _dataContext.Database
                            .BeginTransactionAsync(cancellationToken);

                    // --------------------------------------------------
                    // 5️ Create Identity User
                    // --------------------------------------------------
                    var newUser = new ApplicationUser
                    {
                        UserName = command.Email,
                        Email = command.Email,
                        IsActive = true
                    };

                    var createUserResult =
                        await _userManager.CreateAsync(
                            newUser,
                            command.Password);

                    if (!createUserResult.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            createUserResult.Errors
                                .Select(e => e.Description));

                        return _errorHandlingService
                            .HandleResourceCreationFailed<ResponseWithTokensDto>(
                                "User",
                                nameof(RegisterUserCommandHandler),
                                errors,
                                command.CorrelationId);
                    }

                    // --------------------------------------------------
                    // 6️ Create Domain UserProfile
                    // --------------------------------------------------
                    var userProfile = UserProfile.Create(
                        Guid.NewGuid(),
                        newUser.Id,
                        BasicInformation.Create(
                            command.FirstName,
                            command.LastName,
                            command.DateOfBirth));

                    await _dataContext.UserProfiles
                        .AddAsync(userProfile, cancellationToken);

                    // --------------------------------------------------
                    // 7️ Assign Default Role
                    // --------------------------------------------------
                    var roleResult =
                        await _userManager.AddToRoleAsync(
                            newUser,
                            DefaultRole);

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            roleResult.Errors
                                .Select(e => e.Description));

                        return _errorHandlingService
                            .HandleResourceCreationFailed<ResponseWithTokensDto>(
                                "Role Assignment",
                                nameof(RegisterUserCommandHandler),
                                errors,
                                command.CorrelationId);
                    }

                    // --------------------------------------------------
                    // 8️ Generate Tokens
                    // --------------------------------------------------
                    var roles = new List<string> { DefaultRole };

                    var accessToken =
                        _jwtService.GenerateAccessToken(
                            newUser,
                            userProfile,
                            roles);

                    var refreshToken =
                        _jwtService.GenerateRefreshToken();

                    // --------------------------------------------------
                    // 9️ Store Refresh Token
                    // --------------------------------------------------
                    await _dataContext.RefreshTokens
                        .AddAsync(
                            new RefreshToken
                            {
                                Token = refreshToken,
                                IdentityId = newUser.Id,
                                ExpiryDate =
                                    _jwtService.GetRefreshTokenExpiryDate()
                            },
                            cancellationToken);

                    // --------------------------------------------------
                    // 10️ Save All Changes
                    // --------------------------------------------------
                    await _dataContext
                        .SaveChangesAsync(cancellationToken);

                    // --------------------------------------------------
                    // 11️ Commit Transaction
                    // --------------------------------------------------
                    await transaction
                        .CommitAsync(cancellationToken);

                    // --------------------------------------------------
                    // 12️ Success
                    // --------------------------------------------------
                    return OperationResult<ResponseWithTokensDto>.Success(
                        new ResponseWithTokensDto
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            Message = "User registered successfully."
                        });
                });
            }
            catch (DomainException ex)
            {
                return _errorHandlingService
                    .HandleDomainException<ResponseWithTokensDto>(
                        ex,
                        nameof(RegisterUserCommandHandler),
                        command.CorrelationId);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandlingService
                    .HandleCancelationTokenException<ResponseWithTokensDto>(
                        ex,
                        nameof(RegisterUserCommandHandler),
                        command.CorrelationId);
            }
            catch (Exception ex)
            {
                return _errorHandlingService
                    .HandleUnknownExceptions<ResponseWithTokensDto>(
                        ex,
                        nameof(RegisterUserCommandHandler),
                        command.CorrelationId);
            }
        }
    }
}
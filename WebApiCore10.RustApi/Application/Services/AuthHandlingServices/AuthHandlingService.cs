using DoimanDlls.Exceptions;
using DoimanDlls.UserProfiles;
using DoimanDlls.UserProfiles.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedDll.Errors;
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

        public async Task<OperationResult<ResponseWithTokensDto>> 
            RefreshTokenCommandHandler(
                RefreshTokenCommand command, 
                CancellationToken cancellationToken)
        {
            try
            {
                // --------------------------------------------------
                // 1️ Execution Strategy
                // --------------------------------------------------
                var strategy =
                    _dataContext.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async () =>
                {
                    // --------------------------------------------------
                    // 2️ Begin Transaction
                    // --------------------------------------------------
                    await using var transaction =
                        await _dataContext.Database
                            .BeginTransactionAsync(cancellationToken);

                    // --------------------------------------------------
                    // 3️ Find Refresh Token
                    // --------------------------------------------------
                    var refreshToken =
                        await _dataContext.RefreshTokens
                            .FirstOrDefaultAsync(
                                rt => rt.Token == command.RefreshToken,
                                cancellationToken);

                    if (refreshToken is null)
                    {
                        _logger.LogWarning(
                            "Refresh token not found.");

                        return OperationResult<ResponseWithTokensDto>
                            .Failure(
                                new Error(
                                    ErrorCode.Unauthorized,
                                    "Invalid refresh token.",
                                    command.CorrelationId));
                    }

                    // --------------------------------------------------
                    // 4️ Reuse Detection / Expired / Revoked
                    // --------------------------------------------------
                    if (refreshToken.IsUsed ||
                        refreshToken.IsRevoked ||
                        refreshToken.ExpiryDate <= DateTime.UtcNow)
                    {
                        _logger.LogWarning(
                            "Refresh token reuse detected for UserId: {UserId}",
                            refreshToken.IdentityId);

                        // Revoke ALL active refresh tokens for this user
                        var userTokens =
                            await _dataContext.RefreshTokens
                                .Where(rt =>
                                    rt.IdentityId == refreshToken.IdentityId &&
                                    !rt.IsRevoked)
                                .ToListAsync(cancellationToken);

                        foreach (var token in userTokens)
                        {
                            token.IsUsed = true;
                            token.IsRevoked = true;
                        }

                        await _dataContext
                            .SaveChangesAsync(cancellationToken);

                        await transaction
                            .CommitAsync(cancellationToken);

                        return OperationResult<ResponseWithTokensDto>
                            .Failure(
                                new Error(
                                    ErrorCode.Unauthorized,
                                    "Refresh token reuse detected. Please log in again.",
                                    command.CorrelationId));
                    }

                    // --------------------------------------------------
                    // 5️ Find User
                    // --------------------------------------------------
                    var user =
                        await _userManager.FindByIdAsync(
                            refreshToken.IdentityId);

                    var validation =
                        await ValidateUserCanAuthenticate<ResponseWithTokensDto>(
                            user,
                            command.CorrelationId,
                            nameof(RefreshTokenCommandHandler));

                    if (!validation.IsSuccess)
                    {
                        return validation;
                    }

                    // --------------------------------------------------
                    // 6️ Retrieve User Profile
                    // --------------------------------------------------
                    var userProfile =
                        await _dataContext.UserProfiles
                            .FirstOrDefaultAsync(
                                up => up.IdentityID == user!.Id,
                                cancellationToken);

                    if (userProfile is null)
                    {
                        _logger.LogWarning(
                            "UserProfile not found for UserId: {UserId}",
                            user!.Id);

                        return OperationResult<ResponseWithTokensDto>
                            .Failure(
                                new Error(
                                    ErrorCode.NotFound,
                                    "User profile not found.",
                                    command.CorrelationId));
                    }

                    // --------------------------------------------------
                    // 7 Rotate Current Refresh Token
                    // --------------------------------------------------
                    refreshToken.IsUsed = true;
                    refreshToken.IsRevoked = true;

                    // --------------------------------------------------
                    // 8 Generate New Tokens
                    // --------------------------------------------------
                    var roles =
                        (await _userManager.GetRolesAsync(user!))
                            .ToList();

                    var newAccessToken =
                        _jwtService.GenerateAccessToken(
                            user!,
                            userProfile,
                            roles);

                    var newRefreshToken =
                        _jwtService.GenerateRefreshToken();

                    // --------------------------------------------------
                    // 9️ Store New Refresh Token
                    // --------------------------------------------------
                    await _dataContext.RefreshTokens
                        .AddAsync(
                            new RefreshToken
                            {
                                Token = newRefreshToken,
                                IdentityId = user!.Id,
                                ExpiryDate =
                                    _jwtService.GetRefreshTokenExpiryDate()
                            },
                            cancellationToken);

                    // --------------------------------------------------
                    // 10 Save Changes
                    // --------------------------------------------------
                    await _dataContext
                        .SaveChangesAsync(cancellationToken);

                    // --------------------------------------------------
                    // 11 Commit Transaction
                    // --------------------------------------------------
                    await transaction
                        .CommitAsync(cancellationToken);

                    // --------------------------------------------------
                    // 12 Success
                    // --------------------------------------------------
                    return OperationResult<ResponseWithTokensDto>
                        .Success(
                            new ResponseWithTokensDto
                            {
                                AccessToken = newAccessToken,
                                RefreshToken = newRefreshToken,
                                Message = "Token refreshed successfully."
                            });

                });
            }
            catch (DomainException ex)
            {
                return _errorHandlingService
                    .HandleDomainException<ResponseWithTokensDto>(
                        ex,
                        nameof(RefreshTokenCommandHandler),
                        command.CorrelationId);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandlingService
                    .HandleCancelationTokenException<ResponseWithTokensDto>(
                        ex,
                        nameof(RefreshTokenCommandHandler),
                        command.CorrelationId);
            }
            catch (Exception ex)
            {
                return _errorHandlingService
                    .HandleUnknownExceptions<ResponseWithTokensDto>(
                        ex,
                        nameof(RefreshTokenCommandHandler),
                        command.CorrelationId);
            }
        }


        
        
        // -------------------- HELPER METHODS --------------------
        private async Task<OperationResult<T>> ValidateUserCanAuthenticate<T>(ApplicationUser? user, string correlationId, string operationName)
        {
            if (user is null)
            {
                var errorMessage = "Invalid username or password.";
                _logger.LogWarning(errorMessage);
                return OperationResult<T>
                    .Failure(
                        new Error(
                            ErrorCode.Unauthorized,
                            details: errorMessage,
                            correlationId: correlationId));
            }

            if (!user!.IsActive)
            {
                _logger.LogWarning("User {user} is no longer active.", user.Email);
                return OperationResult<T>
                    .Failure(
                        new Error(
                            ErrorCode.Unauthorized,
                            details: $"Access to '{operationName}' was denied due to insufficient permissions.",
                            correlationId: correlationId));
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning($"Account is locked.. Contact Admin..");
                return OperationResult<T>
                    .Failure(
                        new Error(
                            ErrorCode.Unauthorized,
                            details: $"User '{user}' is currently locked. Please try again later or contact support.",
                            correlationId: correlationId));
            }

            return OperationResult<T>.Success(default!);
        }

    }
}
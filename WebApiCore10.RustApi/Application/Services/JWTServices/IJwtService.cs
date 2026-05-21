using DoimanDlls.UserProfiles;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models;

namespace WebApiCore10.RustApi.Application.Services.JWTServices
{
    public interface IJwtService
    {
        string GenerateAccessToken(ApplicationUser User, UserProfile profile, List<string> roles);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiryDate();
    }
}

using DoimanDlls.UserProfiles;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models;

namespace WebApiCore10.RustApi.Application.Services.JWTServices
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateAccessToken(ApplicationUser User, UserProfile profile, List<string> roles)
        {
            var claims = new List<Claim>
            {
                // Add IdentityUser ID
                new Claim(ClaimTypes.NameIdentifier, User.Id),

                // Add IdentityUser username (email)
                new Claim(ClaimTypes.Name, User.UserName!),
                new Claim(ClaimTypes.Email, User.Email!),

                new Claim("IdentityId", User.Id),
                new Claim("UserProfileId", profile.UserProfileID.ToString()),
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            ////// Get the secret key from environment variable
            ////var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            ////_jwtSettings.Key = secretKey!;

            //var secretKey = _jwtSettings.GetSecretKey();

            // Create the security key and signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.GetSecretKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            // Serialize the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetRefreshTokenExpiryDate()
        {
            return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays); // Refresh token expiry
        }
    }
}

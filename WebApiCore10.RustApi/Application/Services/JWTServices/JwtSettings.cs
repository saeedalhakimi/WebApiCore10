namespace WebApiCore10.RustApi.Application.Services.JWTServices
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
        public int RefreshTokenExpiryInDays { get; set; } // For refresh token

        public string GetSecretKey()
        {
            // Get the secret key from environment variable
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");
            }
            return secretKey;
        }
    }
}

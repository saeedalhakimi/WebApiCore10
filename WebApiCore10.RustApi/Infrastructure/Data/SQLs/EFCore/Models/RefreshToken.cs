namespace WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models
{
    public class RefreshToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string IdentityId { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;
    }
}

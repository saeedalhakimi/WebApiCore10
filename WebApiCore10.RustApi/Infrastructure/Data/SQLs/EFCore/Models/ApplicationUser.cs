using Microsoft.AspNetCore.Identity;

namespace WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}

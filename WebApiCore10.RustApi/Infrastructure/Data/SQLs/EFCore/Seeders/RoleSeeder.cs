using Microsoft.AspNetCore.Identity;

namespace WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            [
                "User",
                "Admin"
            ];

            foreach (var role in roles)
            {
                var roleExists =
                    await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }
        }
    }
}

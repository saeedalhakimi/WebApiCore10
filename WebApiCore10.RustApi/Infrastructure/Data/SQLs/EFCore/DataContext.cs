using DoimanDlls.UserProfiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Configurations;
using WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Models;

namespace WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply the RefreshToken configuration
            builder.ApplyConfiguration(new RefreshTokenConfig());
            builder.ApplyConfiguration(new UserProfileConfig());

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.IsActive)
                 .IsRequired()
                 .HasDefaultValue(true);   // SQL default = 1
            });

            base.OnModelCreating(builder);
            // Additional model configuration can go here
        }
    }
}

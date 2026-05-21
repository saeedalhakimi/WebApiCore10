using DoimanDlls.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApiCore10.RustApi.Infrastructure.Data.SQLs.EFCore.Configurations
{
    internal class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.OwnsOne(up => up.BasicInfo);
        }
    }
}

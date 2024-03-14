using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .ToTable("Users", SchemaName.Security)
                .HasKey(x => x.Id);
        }
    }

    public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder
                .ToTable("Roles", SchemaName.Security)
                .HasKey(x => x.Id);
        }
    }

    public class ApplicationRoleClaimConfig : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder
                .ToTable("RoleClaims", SchemaName.Security)
                .HasKey(x => x.Id);
        }
    }

    public class ApplicationUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder
                .ToTable("UserRoles", SchemaName.Security)
                .HasKey(x => new { x.UserId, x.RoleId });
        }
    }

    public class ApplicationUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder
                .ToTable("UserLogins", SchemaName.Security)
                .HasKey(x => x.UserId);
        }
    }

    public class ApplicationUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder
                .ToTable("UserTokens", SchemaName.Security)
                .HasKey(x => new { x.UserId });
        }
    }
}
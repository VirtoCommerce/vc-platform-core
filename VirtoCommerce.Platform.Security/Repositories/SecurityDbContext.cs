using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security.Repositories
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser, Role, string>
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Role>().Ignore(x => x.Permissions);
            builder.Entity<ApplicationUser>().Ignore(x => x.Password);
            builder.Entity<ApplicationUser>().Ignore(x => x.Roles);
            builder.Entity<ApplicationUser>().Property(x => x.UserType).HasMaxLength(64);
            builder.Entity<ApplicationUser>().Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Entity<ApplicationUser>().Property(x => x.Id).HasMaxLength(128);
            builder.Entity<Role>().Property(x => x.Id).HasMaxLength(128);
            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserRole<string>>().Property(x => x.RoleId).HasMaxLength(128);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasMaxLength(128);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);
        }
    }
}

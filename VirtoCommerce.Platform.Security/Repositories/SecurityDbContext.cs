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
        }
    }
}

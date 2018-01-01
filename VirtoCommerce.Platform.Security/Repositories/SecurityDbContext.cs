using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Security.Model;

namespace VirtoCommerce.Platform.Security.Repositories
{
    public class SecurityDbContext : IdentityDbContext<IdentityUser>
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

            // Tables
            builder.Entity<ApplicationUserEntity>().ToTable("PlatformAccount");
            builder.Entity<ApplicationUserEntity>().HasIndex(x => x.UserName).HasName("IX_UserName").IsUnique(true);


            builder.Entity<ApiAccountEntity>().ToTable("PlatformApiAccount");
            builder.Entity<ApiAccountEntity>().HasOne(x => x.Account)
                        .WithMany(x => x.ApiAccounts)
                        .HasForeignKey(x => x.AccountId)
                        .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApiAccountEntity>().HasIndex(x => x.AppId).HasName("IX_AppId").IsUnique(true);

            builder.Entity<RoleEntity>().ToTable("PlatformRole");

            builder.Entity<PermissionEntity>().ToTable("PlatformPermission");

            builder.Entity<RolePermissionEntity>().ToTable("PlatformRolePermission");
            builder.Entity<RolePermissionEntity>().HasOne(x => x.Permission)
                        .WithMany(x => x.RolePermissions)
                        .HasForeignKey(x => x.PermissionId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePermissionEntity>().HasOne(x => x.Role)
                        .WithMany(x => x.RolePermissions)
                        .HasForeignKey(x => x.RoleId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoleAssignmentEntity>().ToTable("PlatformRoleAssignment");
            builder.Entity<RoleAssignmentEntity>().HasOne(x => x.Account)
                        .WithMany(x => x.RoleAssignments)
                        .HasForeignKey(x => x.AccountId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoleAssignmentEntity>().HasOne(x => x.Role)
                        .WithMany()
                        .HasForeignKey(x => x.RoleId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PermissionScopeEntity>().ToTable("PlatformPermissionScope");
            builder.Entity<PermissionScopeEntity>().HasOne(x => x.RolePermission)
                        .WithMany(x => x.Scopes)
                        .HasForeignKey(x => x.RolePermissionId)
                        .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

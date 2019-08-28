using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ContentModule.Data.Model;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class MenuDbContext : DbContextWithTriggers
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options)
            : base(options)
        {
        }

        protected MenuDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region MenuLinkList

            modelBuilder.Entity<MenuLinkListEntity>().ToTable("ContentMenuLinkList").HasKey(x => x.Id);
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<MenuLinkListEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region MenuLink

            modelBuilder.Entity<MenuLinkEntity>().ToTable("ContentMenuLink").HasKey(x => x.Id);
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<MenuLinkEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            modelBuilder.Entity<MenuLinkEntity>()
                .HasOne(m => m.MenuLinkList)
                .WithMany(m => m.MenuLinks)
                .HasForeignKey(m => m.MenuLinkListId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}

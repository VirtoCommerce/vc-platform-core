using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ImageToolsModule.Data.Models;

namespace VirtoCommerce.ImageToolsModule.Data.Repositories
{
    public class ThumbnailDbContext : DbContextWithTriggers
    {
        public ThumbnailDbContext(DbContextOptions<ThumbnailDbContext> options)
            : base(options)
        {
        }

        protected ThumbnailDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ThumbnailTask

            modelBuilder.Entity<ThumbnailTaskEntity>().ToTable("ThumbnailTask").HasKey(t => t.Id);
            modelBuilder.Entity<ThumbnailTaskEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ThumbnailTaskEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<ThumbnailTaskEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region ThumbnailOption

            modelBuilder.Entity<ThumbnailOptionEntity>().ToTable("ThumbnailOption").HasKey(t => t.Id);
            modelBuilder.Entity<ThumbnailOptionEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<ThumbnailOptionEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<ThumbnailOptionEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region ThumbnailTaskOption

            modelBuilder.Entity<ThumbnailTaskOptionEntity>().ToTable("ThumbnailTaskOption").HasKey(x => x.Id);
            modelBuilder.Entity<ThumbnailTaskOptionEntity>().Property(x => x.Id).HasMaxLength(128);

            modelBuilder.Entity<ThumbnailTaskOptionEntity>()
                .HasOne(x => x.ThumbnailTask)
                .WithMany(t => t.ThumbnailTaskOptions)
                .HasForeignKey(x => x.ThumbnailTaskId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThumbnailTaskOptionEntity>()
                .HasOne(x => x.ThumbnailOption)
                .WithMany()
                .HasForeignKey(x => x.ThumbnailOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}

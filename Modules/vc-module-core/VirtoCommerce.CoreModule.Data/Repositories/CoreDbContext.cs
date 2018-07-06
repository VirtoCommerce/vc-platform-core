using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Data.Model;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public class CoreDbContext : DbContextWithTriggers
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeoUrlKeywordEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SeoUrlKeywordEntity>().ToTable("SeoUrlKeyword");
            modelBuilder.Entity<SeoUrlKeywordEntity>().HasIndex(x => new { x.Keyword, x.StoreId }).HasName("IX_KeywordStoreId");
            modelBuilder.Entity<SeoUrlKeywordEntity>().HasIndex(x => new { x.ObjectId, x.ObjectType }).HasName("IX_ObjectIdAndObjectType");

            modelBuilder.Entity<SequenceEntity>().HasKey(x => x.ObjectType);
            modelBuilder.Entity<SequenceEntity>().ToTable("Sequence");


            modelBuilder.Entity<CurrencyEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<CurrencyEntity>().ToTable("Currency");
            modelBuilder.Entity<CurrencyEntity>().HasIndex(x => x.Code).HasName("IX_Code");

            modelBuilder.Entity<PackageTypeEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PackageTypeEntity>().ToTable("PackageType");

            base.OnModelCreating(modelBuilder);
        }
    }
}

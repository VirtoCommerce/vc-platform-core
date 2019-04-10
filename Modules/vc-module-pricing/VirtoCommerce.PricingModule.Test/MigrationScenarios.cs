using VirtoCommerce.PricingModule.Data.Repositories;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class MigrationScenarios //: MigrationsTestBase
    {
        [Fact]
        [Trait("Category", "CI")]
        public void Can_create_pricing_new_database()
        {
            // TODO: apparently, MigrationsTestBase is not implemented yet...

            //DropDatabase();

            //var migrator = CreateMigrator<Configuration>();

            //using (var context = CreateContext<PricingRepositoryImpl>())
            //{
            //    context.Database.CreateIfNotExists();
            //    new SetupDatabaseInitializer<PricingRepositoryImpl, Configuration>().InitializeDatabase(context);
            //    Assert.Equal(0, context.Pricelists.Count());
            //}

            //// remove all migrations
            //migrator.Update("0");
            //Assert.False(TableExists("Pricelist"));
            //var existTables = Info.Tables.Any();
            //Assert.False(existTables);

            //DropDatabase();
        }
    }
}

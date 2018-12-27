using VirtoCommerce.OrdersModule.Data.Repositories;
using Xunit;

namespace VirtoCommerce.OrdersModule.Tests
{
    public class MigrationScenarios //: MigrationsTestBase
    {
        [Fact]
        [Trait("Category", "CI")]
        public void Can_create_order_new_database()
        {
            //DropDatabase();

            //var migrator = CreateMigrator<Configuration>();

            //using (var context = CreateContext<OrderRepositoryImpl>())
            //{
            //    context.Database.CreateIfNotExists();
            //    new SetupDatabaseInitializer<OrderRepositoryImpl, Configuration>().InitializeDatabase(context);
            //    Assert.Equal(0, context.CustomerOrders.Count());
            //}

            //// remove all migrations
            //migrator.Update("0");
            //Assert.False(TableExists("OrderLineItem"));
            //var existTables = Info.Tables.Any();
            //Assert.False(existTables);

            //DropDatabase();
        }
    }
}

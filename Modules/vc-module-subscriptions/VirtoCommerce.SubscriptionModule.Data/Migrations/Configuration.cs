using System.Data.Entity.Migrations;

namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<SubscriptionModule.Data.Repositories.SubscriptionRepositoryImpl>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SubscriptionModule.Data.Repositories.SubscriptionRepositoryImpl context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

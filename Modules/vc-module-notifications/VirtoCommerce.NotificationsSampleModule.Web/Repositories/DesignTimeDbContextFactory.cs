using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.NotificationsSampleModule.Web.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TwitterNotificationDbContext>
    {
        public TwitterNotificationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TwitterNotificationDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new TwitterNotificationDbContext(builder.Options);
        }
    }
}

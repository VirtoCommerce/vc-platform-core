using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.InventoryModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<InventoryDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new InventoryDbContext(builder.Options);
        }
    }
}

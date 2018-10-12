using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerSampleModule.Web.Model;
using VirtoCommerce.CustomerSampleModule.Web.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CustomerSampleModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Customer") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CustomerSampleDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<ICustomerRepository, CustomerSampleRepositoryImpl>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            AbstractTypeFactory<MemberEntity>.RegisterType<SupplierEntity>();
            AbstractTypeFactory<Member>.RegisterType<Supplier>().MapToType<SupplierEntity>();

            AbstractTypeFactory<Member>.OverrideType<Contact, Contact2>().MapToType<Contact2Entity>();
            AbstractTypeFactory<MemberEntity>.OverrideType<ContactEntity, Contact2Entity>();
            AbstractTypeFactory<MembersSearchCriteria>.RegisterType<Contact2SearchCriteria>();

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<CustomerSampleDbContext>())
                {
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                }
            }
        }

        public void Uninstall()
        {
        }
    }
}

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smidge;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Settings;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Modules.Extensions;
using VirtoCommerce.Platform.Security;
using VirtoCommerce.Platform.Security.Repositories;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Middelware;

namespace VirtoCommerce.Platform.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<DemoOptions>(Configuration.GetSection("VirtoCommerce"));

            PlatformVersion.CurrentVersion = SemanticVersion.Parse(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion);

            var mvcBuilder = services.AddMvc();
            services.AddModules(mvcBuilder, options =>
            {
                options.DiscoveryPath = HostingEnvironment.MapPath(@"~/Modules");
                options.ProbingPath = HostingEnvironment.MapPath("~/App_Data/Modules");
                options.VirtualPath = "~/Modules";
            }
            );
            services.AddExternalModules(options =>
            {
                options.ModulesManifestUrl = new Uri(@"http://virtocommerce.blob.core.windows.net/sample-data");
            });

            services.AddIdentity<ApplicationUser, Role>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            // Add memory cache services
            services.AddMemoryCache();
            //Add Smidge runtime bundling library configuration
            services.AddSmidge(Configuration.GetSection("smidge"));

            services.AddPlatformServices(Configuration);
            services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("VirtoCommerce")));

            services.AddScoped<IUserNameResolver, HttpContextUserResolver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseVirtualFolders(folderOptions =>
            {
                folderOptions.Items.Add(PathString.FromUriComponent("/$(Platform)/Scripts"), "/js");
                var localModules = app.ApplicationServices.GetRequiredService<ILocalModuleCatalog>().Modules;
                foreach (var module in localModules.OfType<ManifestModuleInfo>())
                {
                    folderOptions.Items.Add(PathString.FromUriComponent($"/Modules/$({ module.ModuleName })"), HostingEnvironment.GetRelativePath("~/Modules", module.FullPhysicalPath));
                }
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            //Force migrations
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var platformDbContext = serviceScope.ServiceProvider.GetRequiredService<PlatformDbContext>();
                platformDbContext.Database.Migrate();
            }

            //Using Smidge runtime bundling library for bundling modules js and css files
            app.UseSmidge(bundles =>
            {
                app.UseModulesContent(bundles);
            });
          
            app.UseDbTriggers();
            //Register platform settings
            app.UsePlatformSettings();
            app.UseModules();
          
         
        }
    }
}

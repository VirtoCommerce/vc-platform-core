using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smidge;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Modules.Extensions;
using VirtoCommerce.Platform.Web.Extensions;

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

            //Add Smidge runtime bundling library configuration
            services.AddSmidge(Configuration.GetSection("smidge"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            //Using Smidge runtime bundling library for bundling modules js and css files
            app.UseSmidge(bundles =>
            {
                app.UseModulesContent(bundles);

            });
        }
    }
}

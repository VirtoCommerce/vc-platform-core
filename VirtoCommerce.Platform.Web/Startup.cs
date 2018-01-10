using System;
using System.IO;
using System.Linq;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Smidge;
using Swashbuckle.AspNetCore.Swagger;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Modules.Extensions;
using VirtoCommerce.Platform.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.Platform.Security.Repositories;
using VirtoCommerce.Platform.Security.Services;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Middelware;
using VirtoCommerce.Platform.Web.Swagger;

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
                options.ModulesManifestUrl = new Uri(@"https://raw.githubusercontent.com/VirtoCommerce/vc-modules/master/modules.json");
            });

            services.AddDbContext<SecurityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("VirtoCommerce"));
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            services.AddIdentity<ApplicationUser, Role>()
                    .AddEntityFrameworkStores<SecurityDbContext>()
                    .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

        
           // Register the OAuth2 validation handler.
            services.AddAuthentication().AddOAuthValidation();


            // Register the OpenIddict services.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<SecurityDbContext>();


                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Enable the authorization, logout, token and userinfo endpoints.
                options.EnableTokenEndpoint("/connect/token")
                       .EnableUserinfoEndpoint("/api/security/userinfo");

                // Note: the Mvc.Client sample only uses the code flow and the password flow, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow()
                       .AllowClientCredentialsFlow();

                // Make the "client_id" parameter mandatory when sending a token request.
                options.RequireClientIdentification();

                // When request caching is enabled, authorization and logout requests
                // are stored in the distributed cache by OpenIddict and the user agent
                // is redirected to the same page with a single parameter (request_id).
                // This allows flowing large OpenID Connect requests even when using
                // an external authentication provider like Google, Facebook or Twitter.
                options.EnableRequestCaching();

                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();

                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following lines are required:
                //
                options.UseJsonWebTokens();
                //TODO: Replace to X.509 certificate
                options.AddEphemeralSigningKey();
            });


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


            services.AddAuthorization();
            // register the AuthorizationPolicyProvider which dynamically registers authorization policies for each permission defined in module manifest
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            //Platform authorization handler for policies based on permissions 
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Add memory cache services
            services.AddMemoryCache();
            //Add Smidge runtime bundling library configuration
            services.AddSmidge(Configuration.GetSection("smidge"));
            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { /*Title = "VirtoCommerce Solution REST API documentation",*/ Version = "v1" });
                c.TagActionsBy(api => api.GroupByModuleName(services));
                c.DocInclusionPredicate((docName, api) => true);
                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteProperties();
                c.IgnoreObsoleteActions();
                c.OperationFilter<FileResponseTypeFilter>();
                c.OperationFilter<OptionalParametersFilter>();
                c.MapType<object>(() => new Schema { Type = "object" });
                var xmlCommentsDirectoryPaths = new[]
                {
                    HostingEnvironment.MapPath("~/App_Data/Modules"),
                    AppContext.BaseDirectory
                };
                c.AddModulesXmlComments(xmlCommentsDirectoryPaths);
            });

            services.AddPlatformServices(Configuration);
          
            services.AddScoped<IUserNameResolver, HttpContextUserResolver>();
            services.AddSingleton<IPermissionsProvider, DefaultPermissionProvider>();
            services.AddScoped<IRoleSearchService, RoleSearchService>();
            services.AddScoped<IUserSearchService, UserSearchService>();
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

            //Return all errors as Json response
            app.UseMiddleware<ApiErrorWrappingMiddleware>();

            app.UseVirtualFolders(folderOptions =>
            {
                folderOptions.Items.Add(PathString.FromUriComponent("/$(Platform)/Scripts"), "/js");
                var localModules = app.ApplicationServices.GetRequiredService<ILocalModuleCatalog>().Modules;
                foreach (var module in localModules.OfType<ManifestModuleInfo>())
                {
                    folderOptions.Items.Add(PathString.FromUriComponent($"/Modules/$({ module.ModuleName })"), HostingEnvironment.GetRelativePath("~/Modules", module.FullPhysicalPath));
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions
            {
                RequestPath = "/docs",
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "swagger")),
                EnableDefaultFiles = true //serve index.html at /{ options.RoutePrefix }/
            });

            app.UseAuthentication();

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
                var securityDbContext = serviceScope.ServiceProvider.GetRequiredService<SecurityDbContext>();
                platformDbContext.Database.Migrate();
                securityDbContext.Database.Migrate();
            }

            //Using Smidge runtime bundling library for bundling modules js and css files
            app.UseSmidge(bundles =>
            {
                app.UseModulesContent(bundles);
            });
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Explore");
                c.RoutePrefix = "docs";
                c.EnabledValidator();
            });

            app.UseDbTriggers();
            //Register platform settings
            app.UsePlatformSettings();
            app.UseModules();
            //Register platform permissions
            app.UsePlatformPermissions();
            app.UsePlatformServices();
            //Seed default users
            app.UseDefaultUsersAsync().GetAwaiter().GetResult();
        }
    }
}

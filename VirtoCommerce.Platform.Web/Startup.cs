using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Smidge;
using Smidge.Nuglify;
using Swashbuckle.AspNetCore.Swagger;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Notifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.PushNotifications;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Modules;
using VirtoCommerce.Platform.Modules.Extensions;
using VirtoCommerce.Platform.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.Platform.Security.Extensions;
using VirtoCommerce.Platform.Security.Repositories;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Hangfire;
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
            services.Configure<HangfireOptions>(Configuration.GetSection("VirtoCommerce:Jobs"));
            
            PlatformVersion.CurrentVersion = SemanticVersion.Parse(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion);

            services.AddPlatformServices(Configuration);

            var mvcBuilder = services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.Error = (sender, args) =>
                    {
                        var log = services.BuildServiceProvider().GetService<ILogger<JsonSerializerSettings>>();
                        log.LogError(args.ErrorContext.Error, args.ErrorContext.Error.Message);
                    };
                }
            );
            var modulesDiscoveryPath = Path.GetFullPath("Modules");
            services.AddModules(mvcBuilder, options =>
            {
                options.DiscoveryPath = modulesDiscoveryPath;
                options.ProbingPath = "App_Data/Modules";
            });
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

            //always  return 401 instead of 302 for unauthorized  requests
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });


            services.AddAuthorization();
            // register the AuthorizationPolicyProvider which dynamically registers authorization policies for each permission defined in module manifest
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            //Platform authorization handler for policies based on permissions 
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // Add memory cache services
            services.AddMemoryCache();
            //Add Smidge runtime bundling library configuration
            services.AddSmidge(Configuration.GetSection("smidge"), new PhysicalFileProvider(modulesDiscoveryPath));
            services.AddSmidgeNuglify();

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "VirtoCommerce Solution REST API documentation", Version = "v1" });
                c.TagActionsBy(api => api.GroupByModuleName(services));
                c.DocInclusionPredicate((docName, api) => true);
                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteProperties();
                c.IgnoreObsoleteActions();
                c.OperationFilter<FileResponseTypeFilter>();
                c.OperationFilter<OptionalParametersFilter>();
                c.OperationFilter<TagsFilter>();
                c.DocumentFilter<TagsFilter>();
                c.MapType<object>(() => new Schema { Type = "object" });
                c.AddModulesXmlComments(services);
            });

            //Add SignalR for push notifications
            services.AddSignalR();
            
            

            services.AddSecurityServices();

            var assetConnectionString = BlobConnectionString.Parse(Configuration.GetConnectionString("AssetsConnectionString"));
            //TODO: Azure blob storage
            if (assetConnectionString.Provider.EqualsInvariant("AzureBlobStorage"))
            {
                //var azureBlobOptions = new AzureBlobContentOptions();
                //Configuration.GetSection("VirtoCommerce:AzureBlobStorage").Bind(azureBlobOptions);

                //services.AddAzureBlobContent(options =>
                //{
                //    options.Container = contentConnectionString.RootPath;
                //    options.ConnectionString = contentConnectionString.ConnectionString;
                //    options.PollForChanges = azureBlobOptions.PollForChanges;
                //    options.ChangesPoolingInterval = azureBlobOptions.ChangesPoolingInterval;
                //});
            }
            else
            {
                services.AddFileSystemBlobProvider(options =>
                {
                    options.StoragePath = HostingEnvironment.MapPath(assetConnectionString.RootPath);
                    options.BasePublicUrl = assetConnectionString.PublicUrl;
                });
            }

            var hangfireOptions = new HangfireOptions();
            Configuration.GetSection("VirtoCommerce:Hangfire").Bind(hangfireOptions);
            if (hangfireOptions.JobStorageType == HangfireJobStorageType.SqlServer)
            {
                services.AddHangfire(config => config.UseSqlServerStorage(Configuration.GetConnectionString("VirtoCommerce")));
            }
            else
            {
                services.AddHangfire(config => config.UseMemoryStorage() );
            }
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

            app.UseStaticFiles();

            //Handle all requests like a $(Platform) and Modules/$({ module.ModuleName }) as static files in correspond folder
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(env.MapPath("~/js")),
                RequestPath = new PathString($"/$(Platform)/Scripts")
            });
            var localModules = app.ApplicationServices.GetRequiredService<ILocalModuleCatalog>().Modules;
            foreach (var module in localModules.OfType<ManifestModuleInfo>())
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(module.FullPhysicalPath),
                    RequestPath = new PathString($"/Modules/$({ module.ModuleName })")
                });
            }

         
            app.UseDefaultFiles();


            //register swagger content
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
            app.UseSmidgeNuglify();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v1/docs.json", "Explore");
                c.RoutePrefix = "docs";
                c.EnabledValidator();
            });

            app.UseDbTriggers();
            //Register platform settings
            app.UsePlatformSettings();
            app.UseModules();
            //Register platform permissions
            app.UsePlatformPermissions();

            //Setup SignalR hub
            app.UseSignalR(routes =>
            {
                routes.MapHub<PushNotificationHub>("pushNotificationHub");
            });

            //Seed default users
            app.UseDefaultUsersAsync().GetAwaiter().GetResult();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new [] { new HangfireAuthorizationHandler() }});
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                // Create some queues for job prioritization.
                // Normal equals 'default', because Hangfire depends on it.
                Queues = new[] { JobPriority.High, JobPriority.Normal, JobPriority.Low }
            });
            
        }
    }
}

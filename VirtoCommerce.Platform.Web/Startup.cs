using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Assets.AzureBlobStorage.Extensions;
using VirtoCommerce.Platform.Assets.FileSystem;
using VirtoCommerce.Platform.Assets.FileSystem.Extensions;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Modularity;
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
using VirtoCommerce.Platform.Security.Services;
using VirtoCommerce.Platform.Web.Azure;
using VirtoCommerce.Platform.Web.Extensions;
using VirtoCommerce.Platform.Web.Hangfire;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.JsonConverters;
using VirtoCommerce.Platform.Web.Middleware;
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
            // This custom provider allows able to use just [Authorize] instead of having to define [Authorize(AuthenticationSchemes = "Bearer")] above every API controller
            // without this Bearer authorization will not work
            services.AddSingleton<IAuthenticationSchemeProvider, CustomAuthenticationSchemeProvider>();

            services.Configure<PlatformOptions>(Configuration.GetSection("VirtoCommerce"));
            services.Configure<HangfireOptions>(Configuration.GetSection("VirtoCommerce:Jobs"));

            PlatformVersion.CurrentVersion = SemanticVersion.Parse(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion);

            services.AddPlatformServices(Configuration);
            services.AddSecurityServices();

            var mvcBuilder = services.AddMvc(mvcOptions =>
                {
                    // NOTE: combining multiple Authorize attributes when using a custom IAuthorizationPolicyProvider
                    //       with ASP.NET Core MVC 2.1 causes an ArgumentNullException when calling an action.
                    //       For more information, please see https://github.com/aspnet/Mvc/issues/7809
                    //
                    // Currently this issue affects following controllers:
                    // - VirtoCommerce.Platform.Web.Controllers.Api.DynamicPropertiesController
                    // - VirtoCommerce.SitemapsModule.Web.Controllers.Api.SitemapsModuleApiController
                    // - probably some other controllers in modules not ported to VC Platform 3.x yet...
                    //
                    // This issue is fixed in ASP.NET Core MVC 2.2. The following line is a workaround for 2.1.
                    // TODO: remove the following workaround after migrating to ASP.NET Core MVC 2.2
                    mvcOptions.AllowCombiningAuthorizeFilters = false;
                }
            )
            .AddJsonOptions(options =>
                {
                    //Next line needs to represent custom derived types in the resulting swagger doc definitions. Because default SwaggerProvider used global JSON serialization settings
                    //we should register this converter globally.
                    options.SerializerSettings.ContractResolver = new PolymorphJsonContractResolver();
                    //Next line allow to use polymorph types as parameters in API controller methods
                    options.SerializerSettings.Converters.Add(new PolymorphJsonConverter());
                    options.SerializerSettings.Converters.Add(new ModuleIdentityJsonConverter());
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.Formatting = Formatting.None;

                    options.SerializerSettings.Error += (sender, args) =>
                    {
                        // Expose any JSON serialization exception as HTTP error
                        throw new JsonException(args.ErrorContext.Error.Message);
                    };
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<SecurityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("VirtoCommerce"));
                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication().AddCookie();
            services.AddSecurityServices(options =>
            {
                options.NonEditableUsers = new[] { "admin" };
            });

            services.AddIdentity<ApplicationUser, Role>()
                    .AddEntityFrameworkStores<SecurityDbContext>()
                    .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });
            var azureAdSection = Configuration.GetSection("AzureAd");


            if (azureAdSection.GetChildren().Any())
            {
                var options = new AzureAdOptions();
                azureAdSection.Bind(options);

                if (options.Enabled)
                {
                    //TODO: Need to check how this influence to OpennIddict Reference tokens activated by this line below  AddValidation(options => options.UseReferenceTokens());
                    var auth = services.AddAuthentication().AddOAuthValidation();
                    auth.AddOpenIdConnect(options.AuthenticationType, options.AuthenticationCaption,
                        openIdConnectOptions =>
                        {
                            openIdConnectOptions.ClientId = options.ApplicationId;
                            openIdConnectOptions.Authority = $"{options.AzureAdInstance}{options.TenantId}";
                            openIdConnectOptions.UseTokenLifetime = true;
                            openIdConnectOptions.RequireHttpsMetadata = false;
                            openIdConnectOptions.SignInScheme = IdentityConstants.ExternalScheme;
                        });
                }
            }
            services.Configure<Core.Security.AuthorizationOptions>(Configuration.GetSection("Authorization"));
            var authorizationOptions = Configuration.GetSection("Authorization").Get<Core.Security.AuthorizationOptions>();
            // Register the OpenIddict services.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                        .UseDbContext<SecurityDbContext>();
                }).AddServer(options =>
                {
                    // Register the ASP.NET Core MVC binder used by OpenIddict.
                    // Note: if you don't call this method, you won't be able to
                    // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                    options.UseMvc();

                    // Enable the authorization, logout, token and userinfo endpoints.
                    options.EnableTokenEndpoint("/connect/token")
                        .EnableUserinfoEndpoint("/api/security/userinfo");

                    // Note: the Mvc.Client sample only uses the code flow and the password flow, but you
                    // can enable the other flows if you need to support implicit or client credentials.
                    options.AllowPasswordFlow()
                        .AllowRefreshTokenFlow();

                    options.SetRefreshTokenLifetime(authorizationOptions.RefreshTokenLifeTime);
                    options.SetAccessTokenLifetime(authorizationOptions.AccessTokenLifeTime);

                    options.AcceptAnonymousClients();

                    // Configure Openiddict to issues new refresh token for each token refresh request.
                    options.UseRollingTokens();

                    // Make the "client_id" parameter mandatory when sending a token request.
                    //options.RequireClientIdentification();

                    // When request caching is enabled, authorization and logout requests
                    // are stored in the distributed cache by OpenIddict and the user agent
                    // is redirected to the same page with a single parameter (request_id).
                    // This allows flowing large OpenID Connect requests even when using
                    // an external authentication provider like Google, Facebook or Twitter.
                    options.EnableRequestCaching();

                    options.UseReferenceTokens();
                    options.DisableScopeValidation();

                    // During development, you can disable the HTTPS requirement.
                    if (HostingEnvironment.IsDevelopment())
                    {
                        options.DisableHttpsRequirement();
                    }

                    // Note: to use JWT access tokens instead of the default
                    // encrypted format, the following lines are required:
                    //
                    //options.UseJsonWebTokens();
                    //TODO: Replace to X.509 certificate
                    //options.AddEphemeralSigningKey();
                }).AddValidation(options => options.UseReferenceTokens());

            services.Configure<IdentityOptions>(Configuration.GetSection("IdentityOptions"));

            //always  return 401 instead of 302 for unauthorized  requests
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                };
            });

            services.AddAuthorization();
            // register the AuthorizationPolicyProvider which dynamically registers authorization policies for each permission defined in module manifest
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            //Platform authorization handler for policies based on permissions 
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            // Default password validation service implementation
            services.AddScoped<IPasswordCheckService, PasswordCheckService>();

            var modulesDiscoveryPath = Path.GetFullPath("Modules");
            services.AddModules(mvcBuilder, options =>
            {
                options.DiscoveryPath = modulesDiscoveryPath;
                options.ProbingPath = "App_Data/Modules";
            });

            services.Configure<ExternalModuleCatalogOptions>(Configuration.GetSection("ExternalModules"));
            services.AddExternalModules();

            // Add memory cache services
            services.AddMemoryCache();

            // Register the Swagger generator
            services.AddSwagger();

            //Add SignalR for push notifications
            services.AddSignalR();

            var assetsProvider = Configuration.GetSection("Assets:Provider").Value;
            if (assetsProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                services.Configure<AzureBlobOptions>(Configuration.GetSection("Assets:AzureBlobStorage"));
                services.AddAzureBlobProvider();
            }
            else
            {
                services.Configure<FileSystemBlobOptions>(Configuration.GetSection("Assets:FileSystem"));
                services.AddFileSystemBlobProvider(options =>
                {
                    options.RootPath = HostingEnvironment.MapPath(options.RootPath);
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
                services.AddHangfire(config => config.UseMemoryStorage());
            }

            JobHelper.SetSerializerSettings(new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new ModuleIdentityJsonConverter() },
                NullValueHandling = NullValueHandling.Ignore
            });
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
                app.UseHsts();
            }

            //Return all errors as Json response
            app.UseMiddleware<ApiErrorWrappingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //Force migrations
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var platformDbContext = serviceScope.ServiceProvider.GetRequiredService<PlatformDbContext>();
                platformDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName("Platform"));
                platformDbContext.Database.Migrate();

                var securityDbContext = serviceScope.ServiceProvider.GetRequiredService<SecurityDbContext>();
                securityDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName("Security"));
                securityDbContext.Database.Migrate();
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAuthorizationHandler() } });
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                // Create some queues for job prioritization.
                // Normal equals 'default', because Hangfire depends on it.
                Queues = new[] { JobPriority.High, JobPriority.Normal, JobPriority.Low }
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
                routes.MapHub<PushNotificationHub>("/pushNotificationHub");
            });

            //Seed default users
            app.UseDefaultUsersAsync().GetAwaiter().GetResult();
        }
    }
}

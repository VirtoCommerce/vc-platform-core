using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.SearchPhraseParsing;
using VirtoCommerce.SearchModule.Data.Services;
using VirtoCommerce.SearchModule.Web.BackgroundJobs;

namespace VirtoCommerce.SearchModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISearchPhraseParser, SearchPhraseParser>();
            serviceCollection.AddSingleton<IIndexingWorker>(context =>
            {
                var settingsManager = context.GetService<ISettingsManager>();
                if (settingsManager.GetValue(ModuleConstants.Settings.IndexingJobs.ScaleOut.Name, false))
                {
                    return new HangfireIndexingWorker
                    {
                        ThrottleQueueCount = settingsManager.GetValue(ModuleConstants.Settings.IndexingJobs.MaxQueueSize.Name, 25)
                    };
                }
                else
                {
                    return null;
                }
            });

            serviceCollection.AddSingleton<IIndexingManager, IndexingManager>();
            serviceCollection.AddSingleton<IndexProgressHandler>();

            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            serviceCollection.AddOptions<SearchOptions>().Bind(configuration.GetSection("Search")).ValidateDataAnnotations();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Search",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var settingsManager = appBuilder.ApplicationServices.GetService<ISettingsManager>();
            var scheduleJobs = settingsManager.GetValue(ModuleConstants.Settings.IndexingJobs.Enable.Name, true);
            if (scheduleJobs)
            {
                var cronExpression = settingsManager.GetValue(ModuleConstants.Settings.IndexingJobs.CronExpression.Name, (string)ModuleConstants.Settings.IndexingJobs.CronExpression.DefaultValue);
                RecurringJob.AddOrUpdate<IndexingJobs>(j => j.IndexChangesJob(null, JobCancellationToken.Null), cronExpression);
            }
        }

        public void Uninstall()
        {
        }


    }
}


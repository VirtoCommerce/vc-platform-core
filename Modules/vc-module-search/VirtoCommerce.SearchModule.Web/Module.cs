using System.Linq;
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
            var snapshot = serviceCollection.BuildServiceProvider();
            var settingsManager = snapshot.GetService<ISettingsManager>();

            serviceCollection.AddSingleton<ISearchPhraseParser, SearchPhraseParser>();

            // Allow scale out of indexation through background worker, if opted-in.
            if (settingsManager.GetValue(ModuleConstants.Settings.General.IndexingJobs.ScaleOut.Name, false))
            {
                serviceCollection.AddSingleton<IIndexingWorker>(new HangfireIndexingWorker
                {
                    ThrottleQueueCount = settingsManager.GetValue(ModuleConstants.Settings.General.IndexingJobs.MaxQueueSize.Name, 25)
                });
            }
            else
            {
                serviceCollection.AddSingleton<IIndexingWorker>(c => null);
            }

            serviceCollection.AddSingleton<IIndexingManager, IndexingManager>();

            var configuration = snapshot.GetService<IConfiguration>();
            var searchConnectionString = configuration.GetConnectionString("SearchConnectionString");

            if (string.IsNullOrEmpty(searchConnectionString))
            {
                searchConnectionString = settingsManager.GetValue(ModuleConstants.Settings.General.SearchConnectionString.Name, string.Empty);
            }

            if (!string.IsNullOrEmpty(searchConnectionString))
            {
                serviceCollection.AddSingleton<ISearchConnection>(new SearchConnection(searchConnectionString));
            }

            //TODO delete it after implementation in the modules
            var productIndexingConfiguration = new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Product,
                DocumentSource = new IndexDocumentSource()
            };
            serviceCollection.AddSingleton(new[] { productIndexingConfiguration });

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            ModuleInfo.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Search|General",
                Settings = ModuleConstants.Settings.General.AllSettings.ToArray()
            });


            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IKnownPermissionsProvider>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Search",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var settingsManager = appBuilder.ApplicationServices.GetService<ISettingsManager>();
            var scheduleJobs = settingsManager.GetValue(ModuleConstants.Settings.General.IndexingJobs.Enable.Name, true);
            if (scheduleJobs)
            {
                var cronExpression = settingsManager.GetValue(ModuleConstants.Settings.General.IndexingJobs.CronExpression.Name, ModuleConstants.Settings.General.IndexingJobs.CronExpression.DefaultValue);
                //RecurringJob.AddOrUpdate<IndexingJobs>(j => j.IndexChangesJob(null, JobCancellationToken.Null), cronExpression);
            }
        }

        public void Uninstall()
        {
        }


    }
}


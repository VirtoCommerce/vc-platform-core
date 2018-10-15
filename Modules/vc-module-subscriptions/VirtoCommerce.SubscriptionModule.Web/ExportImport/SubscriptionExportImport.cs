using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;

namespace VirtoCommerce.SubscriptionModule.Web.ExportImport
{
    public sealed class BackupObject
    {
        public BackupObject()
        {
            Subscriptions = new List<Subscription>();
            PaymentPlans = new List<PaymentPlan>();
        }
        public ICollection<PaymentPlan> PaymentPlans { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }


    public sealed class SubscriptionExportImport
    {
        private const int BatchSize = 20;

        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionSearchService _subscriptionSearchService;
        private readonly IPaymentPlanSearchService _paymentPlanSearchService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly JsonSerializer _jsonSerializer;

        public SubscriptionExportImport(ISubscriptionService subscriptionService, ISubscriptionSearchService subscriptionSearchService,
            IPaymentPlanSearchService planSearchService, IPaymentPlanService paymentPlanService)
        {
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _paymentPlanSearchService = planSearchService;
            _paymentPlanService = paymentPlanService;

            _jsonSerializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }


        public async Task DoExportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo("Starting data export");
            progressCallback(progressInfo);

            using (var streamWriter = new StreamWriter(backupStream, Encoding.UTF8))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                await jsonTextWriter.WriteStartObjectAsync();

                var paymentPlanSearchResponse = await _paymentPlanSearchService.SearchPlansAsync(new PaymentPlanSearchCriteria { Take = 0 });

                await jsonTextWriter.WritePropertyNameAsync("PaymentPlans");
                await jsonTextWriter.WriteStartArrayAsync();
                for (int skip = 0; skip < paymentPlanSearchResponse.TotalCount; skip += BatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    paymentPlanSearchResponse = await _paymentPlanSearchService.SearchPlansAsync(new PaymentPlanSearchCriteria
                    {
                        Skip = skip,
                        Take = BatchSize
                    });

                    progressInfo.Description = string.Format("{0} of {1} payment plans loading", Math.Min(skip + BatchSize, paymentPlanSearchResponse.TotalCount), paymentPlanSearchResponse.TotalCount);
                    progressCallback(progressInfo);

                    foreach (var paymentPlan in paymentPlanSearchResponse.Results)
                    {
                        _jsonSerializer.Serialize(jsonTextWriter, paymentPlan);
                    }
                }
                await jsonTextWriter.WriteEndArrayAsync();

                var searchResponse = await _subscriptionSearchService.SearchSubscriptionsAsync(new SubscriptionSearchCriteria
                {
                    Take = 0,
                    ResponseGroup = SubscriptionResponseGroup.Default.ToString()
                });

                await jsonTextWriter.WritePropertyNameAsync("Subscriptions");
                await jsonTextWriter.WriteStartArrayAsync();
                for (int skip = 0; skip < searchResponse.TotalCount; skip += BatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    searchResponse = await _subscriptionSearchService.SearchSubscriptionsAsync(new SubscriptionSearchCriteria
                    {
                        Skip = skip,
                        Take = BatchSize,
                        ResponseGroup = SubscriptionResponseGroup.Default.ToString()
                    });

                    progressInfo.Description = string.Format("{0} of {1} subscriptions loading", Math.Min(skip + BatchSize, searchResponse.TotalCount), searchResponse.TotalCount);
                    progressCallback(progressInfo);

                    foreach (var subscription in searchResponse.Results)
                    {
                        _jsonSerializer.Serialize(jsonTextWriter, subscription);
                    }
                }
                await jsonTextWriter.WriteEndArrayAsync();

                await jsonTextWriter.WriteEndObjectAsync();
                await jsonTextWriter.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var backupObject = backupStream.DeserializeJson<BackupObject>();

            var progressInfo = new ExportImportProgressInfo();
            var totalCount = backupObject.Subscriptions.Count;
            var take = BatchSize;
            for (int skip = 0; skip < totalCount; skip += take)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _subscriptionService.SaveSubscriptionsAsync(backupObject.Subscriptions.Skip(skip).Take(take).ToArray());
                progressInfo.Description = string.Format("{0} of {1} subscriptions imported", Math.Min(skip + take, totalCount), totalCount);
                progressCallback(progressInfo);
            }

            totalCount = backupObject.PaymentPlans.Count;
            for (int skip = 0; skip < totalCount; skip += take)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _paymentPlanService.SavePlansAsync(backupObject.PaymentPlans.Skip(skip).Take(take).ToArray());
                progressInfo.Description = string.Format("{0} of {1} payment plans imported", Math.Min(skip + take, totalCount), totalCount);
                progressCallback(progressInfo);
            }
        }
    }

}

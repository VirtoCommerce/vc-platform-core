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

namespace VirtoCommerce.SubscriptionModule.Data.ExportImport
{
    public sealed class SubscriptionExportImport
    {
        private const int BatchSize = 20;

        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionSearchService _subscriptionSearchService;
        private readonly IPaymentPlanSearchService _paymentPlanSearchService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly JsonSerializer _jsonSerializer;

        public SubscriptionExportImport(ISubscriptionService subscriptionService, ISubscriptionSearchService subscriptionSearchService,
            IPaymentPlanSearchService planSearchService, IPaymentPlanService paymentPlanService, JsonSerializer jsonSerializer)
        {
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _paymentPlanSearchService = planSearchService;
            _paymentPlanService = paymentPlanService;

            _jsonSerializer = jsonSerializer;
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
                for (var skip = 0; skip < paymentPlanSearchResponse.TotalCount; skip += BatchSize)
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
                for (var skip = 0; skip < searchResponse.TotalCount; skip += BatchSize)
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

            var progressInfo = new ExportImportProgressInfo("Preparing for import");
            progressCallback(progressInfo);

            using (var streamReader = new StreamReader(backupStream, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName)
                    {
                        if (jsonReader.Value.ToString() == "PaymentPlans" &&
                            TryReadCollectionOf<PaymentPlan>(jsonReader, out var paymentPlans))
                        {
                            var totalCount = paymentPlans.Count;
                            for (var skip = 0; skip < totalCount; skip += BatchSize)
                            {
                                var currentPaymentPlans = paymentPlans.Skip(skip).Take(BatchSize).ToArray();
                                await _paymentPlanService.SavePlansAsync(currentPaymentPlans);

                                progressInfo.Description = $"{Math.Min(skip + BatchSize, totalCount)} of {totalCount} payment plans have been imported.";
                                progressCallback(progressInfo);
                            }
                        }
                        else if (jsonReader.Value.ToString() == "Subscriptions" &&
                                 TryReadCollectionOf<Subscription>(jsonReader, out var subscriptions))
                        {
                            var totalCount = subscriptions.Count;
                            for (var skip = 0; skip < totalCount; skip += BatchSize)
                            {
                                var currentSubscriptions = subscriptions.Skip(skip).Take(BatchSize).ToArray();
                                await _subscriptionService.SaveSubscriptionsAsync(currentSubscriptions);

                                progressInfo.Description = $"{Math.Min(skip + BatchSize, totalCount)} of {totalCount} subscriptions have been imported.";
                                progressCallback(progressInfo);
                            }
                        }
                    }
                }
            }
        }

        private bool TryReadCollectionOf<TValue>(JsonReader jsonReader, out IReadOnlyCollection<TValue> values)
        {
            jsonReader.Read();
            if (jsonReader.TokenType == JsonToken.StartArray)
            {
                jsonReader.Read();

                var items = new List<TValue>();
                while (jsonReader.TokenType != JsonToken.EndArray)
                {
                    var item = _jsonSerializer.Deserialize<TValue>(jsonReader);
                    items.Add(item);

                    jsonReader.Read();
                }

                values = items;
                return true;
            }

            values = new TValue[0];
            return false;
        }
    }
}

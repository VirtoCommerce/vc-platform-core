using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.InventoryModule.Data.ExportImport
{
    public sealed class InventoryExportImport
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventorySearchService _inventorySearchService;
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;
        private readonly IFulfillmentCenterService _fulfillmentCenterService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _jsonSerializer;

        private int? _batchSize;

        private int BatchSize
        {
            get
            {
                if (_batchSize == null)
                {
                    _batchSize = _settingsManager.GetValue(ModuleConstants.Settings.General.PageSize.Name, 50);
                }

                return (int)_batchSize;
            }
        }

        public InventoryExportImport(IInventoryService inventoryService, IFulfillmentCenterSearchService fulfillmentCenterSearchService,
            IInventorySearchService inventorySearchService, IFulfillmentCenterService fulfillmentCenterService,
            ISettingsManager settingsManager, JsonSerializer jsonSerializer)
        {
            _inventoryService = inventoryService;
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _fulfillmentCenterService = fulfillmentCenterService;
            _inventorySearchService = inventorySearchService;
            _settingsManager = settingsManager;
            _jsonSerializer = jsonSerializer;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "The fulfilmentCenters are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                await writer.WritePropertyNameAsync("FulfillmentCenters");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, BatchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<FulfillmentCenterSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;
                    var searchResult = await _fulfillmentCenterSearchService.SearchCentersAsync(searchCriteria);
                    return (GenericSearchResult<FulfillmentCenter>) searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} FulfillmentCenters have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "The Inventories are loading";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Inventories");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, BatchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<InventorySearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;
                    var searchResult = await _inventorySearchService.SearchInventoriesAsync(searchCriteria);
                    return (GenericSearchResult<InventoryInfo>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} inventories have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "FulfillmentCenters")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<FulfillmentCenter>(_jsonSerializer, BatchSize,
                                items => _fulfillmentCenterService.SaveChangesAsync(items.ToArray()), processedCount =>
                                {
                                    progressInfo.Description = $"{ processedCount } FulfillmentCenters have been imported";
                                    progressCallback(progressInfo);
                                }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Inventories")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<InventoryInfo>(_jsonSerializer, BatchSize,
                                items => _inventoryService.SaveChangesAsync(items.ToArray()), processedCount =>
                                {
                                    progressInfo.Description = $"{ processedCount } Inventories have been imported";
                                    progressCallback(progressInfo);
                                }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}

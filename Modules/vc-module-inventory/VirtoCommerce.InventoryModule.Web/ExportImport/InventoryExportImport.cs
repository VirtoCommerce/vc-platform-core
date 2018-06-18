using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.InventoryModule.Web.ExportImport
{
    public sealed class BackupObject
    {
        public BackupObject()
        {
            InventoryInfos = Array.Empty<InventoryInfo>();
            FulfillmentCenters = Array.Empty<FulfillmentCenter>();
        }
        public InventoryInfo[] InventoryInfos { get; set; }
        public FulfillmentCenter[] FulfillmentCenters { get; set; }
    }

    public sealed class InventoryExportImport
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventorySearchService _inventorySearchService;
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;
        private readonly IFulfillmentCenterService _fulfillmentCenterService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _serializer;

        private int? _batchSize;

        private int BatchSize
        {
            get
            {
                if (_batchSize == null)
                {
                    _batchSize = _settingsManager.GetValue("Inventory.ExportImport.PageSize", 50);
                }

                return (int) _batchSize;
            }
        }

        public InventoryExportImport(
            IInventoryService inventoryService,
            IFulfillmentCenterSearchService fulfillmentCenterSearchService,
            IInventorySearchService inventorySearchService,
            IFulfillmentCenterService fulfillmentCenterService,
            ISettingsManager settingsManager,
            JsonSerializer serializer
            )
        {
            _inventoryService = inventoryService;
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _fulfillmentCenterService = fulfillmentCenterService;
            _inventorySearchService = inventorySearchService;
            _settingsManager = settingsManager;
            _serializer = serializer;
        }

        public async Task DoExport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo { Description = "The fulfilmentCenters are loading" };
            progressCallback(progressInfo);

            //var backupObject = await GetBackupObject(progressCallback);
            using (var sw = new StreamWriter(backupStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                var centers = await _fulfillmentCenterSearchService.SearchCentersAsync(new FulfillmentCenterSearchCriteria { Take = int.MaxValue });
                writer.WritePropertyName("FulfillmentCenterTotalCount");
                writer.WriteValue(centers.TotalCount);

                writer.WritePropertyName("FulfillmentCenters");
                writer.WriteStartArray();

                foreach (var fulfillmentCenter in centers.Results)
                {
                    _serializer.Serialize(writer, fulfillmentCenter);
                }

                writer.WriteEndArray();

                progressInfo.Description = "Evaluation the number of inventory records";
                progressCallback(progressInfo);

                var searchResult = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria { Take = BatchSize });
                var totalCount = searchResult.TotalCount;
                writer.WritePropertyName("InventoriesTotalCount");
                writer.WriteValue(totalCount);

                writer.WritePropertyName("Inventories");
                writer.WriteStartArray();

                for (int i = BatchSize; i < totalCount; i += BatchSize)
                {
                    progressInfo.Description = $"{i} of {totalCount} inventories have been loaded";
                    progressCallback(progressInfo);

                    searchResult = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria { Skip = i, Take = BatchSize });

                    foreach (var inventory in searchResult.Results)
                    {
                        _serializer.Serialize(writer, inventory);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } inventories exported";
                    progressCallback(progressInfo);
                }

                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task DoImport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();
            var fulfillmentCentersTotalCount = 0;
            var inventoriesTotalCount = 0;

            using (var streamReader = new StreamReader(backupStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "FulfillmentCenterTotalCount")
                        {
                            fulfillmentCentersTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "FulfillmentCenters")
                        {
                            var fulfillmentCenters = new List<FulfillmentCenter>();
                            var fulfillmentCenterCount = 0;
                            while (reader.TokenType != JsonToken.EndArray)
                            {
                                var fulfillmentCenter = _serializer.Deserialize<FulfillmentCenter>(reader);
                                fulfillmentCenters.Add(fulfillmentCenter);
                                fulfillmentCenterCount++;

                                reader.Read();
                            }

                            for (int i = 0; i < fulfillmentCenterCount; i += BatchSize)
                            {
                                await _fulfillmentCenterService.SaveChangesAsync(fulfillmentCenters.Skip(i).Take(BatchSize).ToArray());

                                if (fulfillmentCenterCount > 0)
                                {
                                    progressInfo.Description = $"{ i } of { fulfillmentCenterCount } fulfillment centers imported";
                                }
                                else
                                {
                                    progressInfo.Description = $"{ i } fulfillment centers imported";
                                }
                                progressCallback(progressInfo);
                            }

                        }
                        else if (reader.Value.ToString() == "InventoriesTotalCount")
                        {
                            inventoriesTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "Inventories")
                        {
                            var inventories = new List<InventoryInfo>();
                            var inventoryCount = 0;
                            while (reader.TokenType != JsonToken.EndArray)
                            {
                                var inventory = _serializer.Deserialize<InventoryInfo>(reader);
                                inventories.Add(inventory);
                                inventoryCount++;

                                reader.Read();
                            }

                            for (int i = 0; i < inventoryCount; i += BatchSize)
                            {
                                await _inventoryService.SaveChangesAsync(inventories.Skip(i).Take(BatchSize));

                                if (inventoryCount > 0)
                                {
                                    progressInfo.Description =
                                        $"{i} of {inventoryCount} inventories imported";
                                }
                                else
                                {
                                    progressInfo.Description = $"{i} inventories imported";
                                }
                                progressCallback(progressInfo);
                            }
                        }
                    }
                }
            }
        }
    }
}

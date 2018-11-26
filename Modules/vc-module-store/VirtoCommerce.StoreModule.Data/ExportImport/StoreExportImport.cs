using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.StoreModule.Data.ExportImport
{
    public sealed class StoreExportImport : IExportSupport, IImportSupport
    {
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;
        private readonly JsonSerializer _serializer;
        private readonly int BatchSize = 50;

        public StoreExportImport(IStoreService storeService, IStoreSearchService storeSearchService, JsonSerializer jsonSerializer)
        {
            _storeService = storeService;
            _serializer = jsonSerializer;
            _storeSearchService = storeSearchService;
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "The store are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Evaluation the number of store records";
                progressCallback(progressInfo);

                var searchResult = await _storeSearchService.SearchStoresAsync(new StoreSearchCriteria { Take = BatchSize });
                var totalCount = searchResult.TotalCount;
                writer.WritePropertyName("StoreTotalCount");
                writer.WriteValue(totalCount);

                writer.WritePropertyName("Stores");
                writer.WriteStartArray();

                for (int i = BatchSize; i < totalCount; i += BatchSize)
                {
                    progressInfo.Description = $"{i} of {totalCount} stores have been loaded";
                    progressCallback(progressInfo);

                    searchResult = await _storeSearchService.SearchStoresAsync(new StoreSearchCriteria { Skip = i, Take = BatchSize });

                    foreach (var store in searchResult.Results)
                    {
                        _serializer.Serialize(writer, store);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } stores exported";
                    progressCallback(progressInfo);
                }

                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();
            int storeTotalCount = 0;

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "StoresTotalCount")
                        {
                            storeTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "Stores")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var stores = new List<Store>();
                                var storeCount = 0;
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var store = _serializer.Deserialize<Store>(reader);
                                    stores.Add(store);
                                    storeCount++;

                                    reader.Read();
                                }

                                for (int i = 0; i < storeCount; i += BatchSize)
                                {
                                    var batchStores = stores.Skip(i).Take(BatchSize);
                                    foreach (var store in batchStores)
                                    {
                                        await _storeService.SaveChangesAsync(new[] {store});
                                    }

                                    if (storeCount > 0)
                                    {
                                        progressInfo.Description = $"{i} of {storeCount} stores imported";
                                    }
                                    else
                                    {
                                        progressInfo.Description = $"{i} stores imported";
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
}

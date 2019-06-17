using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.StoreModule.Data.ExportImport
{
    public sealed class StoreExportImport
    {
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly int _batchSize = 50;

        public StoreExportImport(IStoreService storeService, IStoreSearchService storeSearchService, JsonSerializer jsonSerializer)
        {
            _storeService = storeService;
            _jsonSerializer = jsonSerializer;
            _storeSearchService = storeSearchService;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "The store are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Stores are started to export";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Stores");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;

                    var searchResult = await _storeSearchService.SearchStoresAsync(searchCriteria);
                    return (GenericSearchResult<Store>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} stores have been exported";
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
                        if (reader.Value.ToString() == "Stores")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Store>(_jsonSerializer, _batchSize, items => _storeService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } stores have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}

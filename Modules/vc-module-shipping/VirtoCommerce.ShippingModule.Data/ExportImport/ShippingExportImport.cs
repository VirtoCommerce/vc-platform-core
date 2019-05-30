using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Data.ExportImport
{
    public class ShippingExportImport
    {
        private readonly IShippingMethodsService _shippingMethodsService;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly int _batchSize = 50;

        public ShippingExportImport(IShippingMethodsService shippingMethodsService, IShippingMethodsSearchService shippingMethodsSearchService, JsonSerializer jsonSerializer)
        {
            _shippingMethodsService = shippingMethodsService;
            _jsonSerializer = jsonSerializer;
            _shippingMethodsSearchService = shippingMethodsSearchService;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "Shipping methods are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Shipping methods are started to export";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("ShippingMethods");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<ShippingMethodsSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;
                    searchCriteria.WithoutTransient = true;

                    var searchResult = await _shippingMethodsSearchService.SearchShippingMethodsAsync(searchCriteria);
                    return (GenericSearchResult<ShippingMethod>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} shipping methods have been exported";
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
                        if (reader.Value.ToString() == "ShippingMethods")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<ShippingMethod>(_jsonSerializer, _batchSize, items => _shippingMethodsService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } shipping methods have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}

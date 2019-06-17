using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.PaymentModule.Data.ExportImport
{
    public sealed class PaymentExportImport
    {
        private readonly IPaymentMethodsService _paymentMethodsService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly int _batchSize = 50;

        public PaymentExportImport(IPaymentMethodsService paymentMethodsService, IPaymentMethodsSearchService paymentMethodsSearchService, JsonSerializer jsonSerializer)
        {
            _paymentMethodsService = paymentMethodsService;
            _jsonSerializer = jsonSerializer;
            _paymentMethodsSearchService = paymentMethodsSearchService;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "The payment methods are loading" };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Payment mthods are started to export";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("PaymentMethods");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchCriteria = AbstractTypeFactory<PaymentMethodsSearchCriteria>.TryCreateInstance();
                    searchCriteria.Take = take;
                    searchCriteria.Skip = skip;

                    var searchResult = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(searchCriteria);
                    return (GenericSearchResult<PaymentMethod>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{processedCount} of {totalCount} payment methods have been exported";
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
                        if (reader.Value.ToString() == "PaymentMethods")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<PaymentMethod>(_jsonSerializer, _batchSize, items => _paymentMethodsService.SaveChangesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } payment methods have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}

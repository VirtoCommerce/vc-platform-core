using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Web.ExportImport
{
    public sealed class PricingExportImport
    {
        private readonly IPricingService _pricingService;
        private readonly IPricingSearchService _pricingSearchService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _jsonSerializer;

        private int? _batchSize;

        public PricingExportImport(IPricingService pricingService, IPricingSearchService pricingSearchService, ISettingsManager settingsManager,
            IOptions<MvcJsonOptions> jsonOptions)
        {
            _pricingService = pricingService;
            _pricingSearchService = pricingSearchService;
            _settingsManager = settingsManager;

            _jsonSerializer = JsonSerializer.Create(jsonOptions.Value.SerializerSettings);
        }

        private int BatchSize
        {
            get
            {
                if (_batchSize == null)
                {
                    _batchSize = _settingsManager.GetValue("Pricing.ExportImport.PageSize", 50);
                }

                return (int)_batchSize;
            }
        }

        public async Task DoExportAsync(Stream backupStream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(backupStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Price lists exporting...";
                progressCallback(progressInfo);

                #region Export price lists
                var totalCount = (await _pricingSearchService.SearchPricelistsAsync(new PricelistSearchCriteria { Take = 0 })).TotalCount;
                writer.WritePropertyName("PricelistsTotalCount");
                writer.WriteValue(totalCount);

                writer.WritePropertyName("Pricelists");
                writer.WriteStartArray();

                for (var i = 0; i < totalCount; i += BatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var searchResponse = await _pricingSearchService.SearchPricelistsAsync(new PricelistSearchCriteria { Skip = i, Take = BatchSize });
                    foreach (var priceList in searchResponse.Results)
                    {
                        priceList.Assignments = null;
                        _jsonSerializer.Serialize(writer, priceList);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } price lists have been exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();
                #endregion

                #region Export price list assignments
                totalCount = (await _pricingSearchService.SearchPricelistAssignmentsAsync(new PricelistAssignmentsSearchCriteria { Take = 0 })).TotalCount;
                writer.WritePropertyName("AssignmentsTotalCount");
                writer.WriteValue(totalCount);

                writer.WritePropertyName("Assignments");
                writer.WriteStartArray();

                for (var i = 0; i < totalCount; i += BatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var searchResponse = await _pricingSearchService.SearchPricelistAssignmentsAsync(new PricelistAssignmentsSearchCriteria { Skip = i, Take = BatchSize });
                    foreach (var assignment in searchResponse.Results)
                    {
                        assignment.Pricelist = null;
                        assignment.DynamicExpression = null;

                        _jsonSerializer.Serialize(writer, assignment);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } price lits assignments have been exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();
                #endregion

                #region Export prices
                totalCount = (await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria { Take = 0 })).TotalCount;
                writer.WritePropertyName("PricesTotalCount");
                writer.WriteValue(totalCount);

                writer.WritePropertyName("Prices");
                writer.WriteStartArray();

                for (var i = 0; i < totalCount; i += BatchSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var searchResponse = await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria { Skip = i, Take = BatchSize });
                    foreach (var price in searchResponse.Results)
                    {
                        price.Pricelist = null;
                        _jsonSerializer.Serialize(writer, price);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(totalCount, i + BatchSize) } of { totalCount } prices have been exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();
                #endregion

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task DoImportAsync(Stream stream, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: add cancellation checks during import

            var progressInfo = new ExportImportProgressInfo();

            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        var readerValue = reader.Value.ToString();

                        if (readerValue == "Pricelists")
                        {
                            reader.Read();

                            var pricelists = _jsonSerializer.Deserialize<Pricelist[]>(reader);

                            progressInfo.Description = $"{pricelists.Count()} price lists importing...";
                            progressCallback(progressInfo);

                            await _pricingService.SavePricelistsAsync(pricelists);
                        }
                        else if (readerValue == "Prices")
                        {
                            reader.Read();

                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var pricesChunk = new List<Price>();

                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var price = _jsonSerializer.Deserialize<Price>(reader);
                                    pricesChunk.Add(price);

                                    reader.Read();

                                    if (pricesChunk.Count >= BatchSize || reader.TokenType == JsonToken.EndArray)
                                    {
                                        await _pricingService.SavePricesAsync(pricesChunk.ToArray());
                                        progressInfo.ProcessedCount += pricesChunk.Count;
                                        progressInfo.Description = $"Prices: {progressInfo.ProcessedCount} have been imported";
                                        progressCallback(progressInfo);

                                        pricesChunk.Clear();
                                    }
                                }
                            }
                        }
                        else if (readerValue == "Assignments")
                        {

                            reader.Read();

                            var assignments = _jsonSerializer.Deserialize<PricelistAssignment[]>(reader);

                            progressInfo.Description = $"{assignments.Count()} assignments importing...";
                            progressCallback(progressInfo);

                            await _pricingService.SavePricelistAssignmentsAsync(assignments);
                        }
                    }
                }
            }
        }
    }
}

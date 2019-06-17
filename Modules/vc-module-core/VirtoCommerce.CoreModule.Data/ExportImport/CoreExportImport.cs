using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Package;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.CoreModule.Web.ExportImport
{

    public sealed class CoreExportImport : IExportSupport, IImportSupport
    {
        private const int _batchSize = 50;
        private readonly JsonSerializer _serializer;
        private readonly ICurrencyService _currencyService;
        private readonly IPackageTypesService _packageTypesService;

        public CoreExportImport(ICurrencyService currencyService, IPackageTypesService packageTypesService, JsonSerializer jsonSerializer)
        {
            _currencyService = currencyService;
            _packageTypesService = packageTypesService;
            _serializer = jsonSerializer;
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, System.Text.Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Currencies exporting...";
                progressCallback(progressInfo);

                var currencyResult = await _currencyService.GetAllCurrenciesAsync();
                writer.WritePropertyName("CurrencyTotalCount");
                writer.WriteValue(currencyResult.Count());

                writer.WritePropertyName("Currencies");
                writer.WriteStartArray();

                foreach (var currency in currencyResult)
                {
                    _serializer.Serialize(writer, currency);
                }

                writer.Flush();
                progressInfo.Description = $"{currencyResult.Count()} currencies exported";
                progressCallback(progressInfo);

                writer.WriteEndArray();

                var packageTypesResult = await _packageTypesService.GetAllPackageTypesAsync();
                writer.WritePropertyName("PackageTypeTotalCount");
                writer.WriteValue(packageTypesResult.Count());

                writer.WritePropertyName("PackageTypes");
                writer.WriteStartArray();

                foreach (var packageType in packageTypesResult)
                {
                    _serializer.Serialize(writer, packageType);
                }

                writer.Flush();
                progressInfo.Description = $"{packageTypesResult.Count()} package types exported";
                progressCallback(progressInfo);
                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();
            var currencyTotalCount = 0;
            var packageTypeTotalCount = 0;

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "CurrencyTotalCount")
                        {
                            currencyTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "Currencies")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var currencies = new List<Currency>();
                                var currencyCount = 0;

                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var currency = _serializer.Deserialize<Currency>(reader);
                                    currencies.Add(currency);
                                    currencyCount++;

                                    reader.Read();
                                }
                                cancellationToken.ThrowIfCancellationRequested();

                                if (currencyCount % _batchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                {
                                    await _currencyService.SaveChangesAsync(currencies.ToArray());
                                    currencies.Clear();

                                    progressInfo.Description = $"{ currencyCount } Currencies imported";

                                    progressCallback(progressInfo);
                                }
                            }
                        }
                        else if (reader.Value.ToString() == "PackageTypeTotalCount")
                        {
                            packageTypeTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "PackageTypes")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var packageTypes = new List<PackageType>();
                                var packageTypeCount = 0;

                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var currency = _serializer.Deserialize<PackageType>(reader);
                                    packageTypes.Add(currency);
                                    packageTypeCount++;

                                    reader.Read();
                                }
                                cancellationToken.ThrowIfCancellationRequested();

                                if (packageTypeCount % _batchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                {
                                    await _packageTypesService.SaveChangesAsync(packageTypes.ToArray());
                                    packageTypes.Clear();

                                    progressInfo.Description = $"{ packageTypeCount } Package Types imported";

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

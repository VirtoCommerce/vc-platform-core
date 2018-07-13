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
        private readonly ISeoService _seoService;

        public CoreExportImport(ICurrencyService currencyService, IPackageTypesService packageTypesService, ISeoService seoService)
        {
            _currencyService = currencyService;
            _packageTypesService = packageTypesService;
            _seoService = seoService;
            _serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
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

                var seoResult = await _seoService.GetAllSeoDuplicatesAsync();
                writer.WritePropertyName("SeoInfoTotalCount");
                writer.WriteValue(seoResult.Count());

                writer.WritePropertyName("SeoInfos");
                writer.WriteStartArray();

                foreach (var seoInfo in seoResult)
                {
                    _serializer.Serialize(writer, seoInfo);
                }

                writer.Flush();
                progressInfo.Description = $"{seoResult.Count()} seo info exported";
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
            var seoInfoTotalCount = 0;

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
                        else if (reader.Value.ToString() == "SeoInfoTotalCount")
                        {
                            seoInfoTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "SeoInfos")
                        {
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var seoInfos = new List<SeoInfo>();
                                var seoInfoCount = 0;

                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var seoInfo = _serializer.Deserialize<SeoInfo>(reader);
                                    seoInfos.Add(seoInfo);
                                    seoInfoCount++;

                                    reader.Read();
                                }
                                cancellationToken.ThrowIfCancellationRequested();

                                if (seoInfoCount % _batchSize == 0 || reader.TokenType == JsonToken.EndArray)
                                {
                                    await _seoService.SaveSeoInfosAsync(seoInfos.ToArray());
                                    seoInfos.Clear();

                                    progressInfo.Description = $"{ seoInfoCount } SeoInfos imported";

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

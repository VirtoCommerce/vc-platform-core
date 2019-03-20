using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Data.ExportImport;

namespace VirtoCommerce.MarketingModule.Data.ExportImport
{
    public class MarketingExportImport
    {
        private readonly JsonSerializer _jsonSerializer;
        private const int _batchSize = 50;
        private readonly IPromotionSearchService _promotionSearchService;
        private readonly IDynamicContentSearchService _dynamicContentSearchService;
        private readonly IPromotionService _promotionService;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly ICouponService _couponService;
        private readonly IPromotionUsageService _promotionUsageService;

        public MarketingExportImport(JsonSerializer jsonSerializer, IPromotionSearchService promotionSearchService, IDynamicContentSearchService dynamicContentSearchService, IPromotionService promotionService, IDynamicContentService dynamicContentService, ICouponService couponService, IPromotionUsageService promotionUsageService)
        {
            _jsonSerializer = jsonSerializer;
            _promotionSearchService = promotionSearchService;
            _dynamicContentSearchService = dynamicContentSearchService;
            _promotionService = promotionService;
            _dynamicContentService = dynamicContentService;
            _couponService = couponService;
            _promotionUsageService = promotionUsageService;
        }

        public async Task DoExportAsync(Stream outStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                progressInfo.Description = "Promotions exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Promotions");

                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _promotionSearchService.SearchPromotionsAsync(new PromotionSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } promotions have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content folders exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentFolders");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _dynamicContentSearchService.SearchFoldersAsync(new DynamicContentFolderSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content folders have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content items exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentItems");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _dynamicContentSearchService.SearchContentItemsAsync(new DynamicContentItemSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content places exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPlaces");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _dynamicContentSearchService.SearchContentPlacesAsync(new DynamicContentPlaceSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content places have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content publications exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPublications");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _dynamicContentSearchService.SearchContentPublicationsAsync(new DynamicContentPublicationSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content publications have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Coupons exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Coupons");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _couponService.SearchCouponsAsync(new CouponSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } coupons have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Usages exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Usages");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, (skip, take) => _promotionUsageService.SearchUsagesAsync(new PromotionUsageSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } usages have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        public async Task DoImportAsync(Stream inputStream, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
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
                        if (reader.Value.ToString() == "Promotions")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Promotion>(_jsonSerializer, _batchSize, items => _promotionService.SavePromotionsAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } promotions have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                        }
                        else if (reader.Value.ToString() == "DynamicContentFolders")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentFolder>(_jsonSerializer, _batchSize, items => _dynamicContentService.SaveFoldersAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentItems")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentItem>(_jsonSerializer, _batchSize, items => _dynamicContentService.SaveContentItemsAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentPlaces")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentPlace>(_jsonSerializer, _batchSize, items => _dynamicContentService.SavePlacesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content places have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentPublications")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentPublication>(_jsonSerializer, _batchSize, items => _dynamicContentService.SavePublicationsAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content publications have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Coupons")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Coupon>(_jsonSerializer, _batchSize, items => _couponService.SaveCouponsAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } coupons have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Usages")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<PromotionUsage>(_jsonSerializer, _batchSize, items => _promotionUsageService.SaveUsagesAsync(items.ToArray()), processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } usages have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}

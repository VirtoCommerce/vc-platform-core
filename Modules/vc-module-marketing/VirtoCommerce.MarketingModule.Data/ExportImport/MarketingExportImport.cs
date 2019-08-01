using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
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
        private readonly IContentPublicationsSearchService _contentPublicationsSearchService;
        private readonly IPromotionService _promotionService;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly ICouponService _couponService;
        private readonly IPromotionUsageService _promotionUsageService;
        private readonly IContentItemsSearchService _contentItemsSearchService;
        private readonly ICouponSearchService _couponSearchService;
        private readonly IContentPlacesSearchService _contentPlacesSearchService;
        private readonly IPromotionUsageSearchService _promotionUsageSearchService;
        private readonly IFolderSearchService _folderSearchService;

        public MarketingExportImport(JsonSerializer jsonSerializer, IPromotionSearchService promotionSearchService,
            IContentPublicationsSearchService contentPublicationsSearchService, IPromotionService promotionService,
            IDynamicContentService dynamicContentService, ICouponService couponService,
            IPromotionUsageService promotionUsageService, IContentItemsSearchService contentItemsSearchService,
            ICouponSearchService couponSearchService, IContentPlacesSearchService contentPlacesSearchService,
            IPromotionUsageSearchService promotionUsageSearchService, IFolderSearchService folderSearchService)
        {
            _jsonSerializer = jsonSerializer;
            _promotionSearchService = promotionSearchService;
            _contentPublicationsSearchService = contentPublicationsSearchService;
            _promotionService = promotionService;
            _dynamicContentService = dynamicContentService;
            _couponService = couponService;
            _promotionUsageService = promotionUsageService;
            _contentItemsSearchService = contentItemsSearchService;
            _couponSearchService = couponSearchService;
            _contentPlacesSearchService = contentPlacesSearchService;
            _promotionUsageSearchService = promotionUsageSearchService;
            _folderSearchService = folderSearchService;
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

                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<Promotion>)await _promotionSearchService.SearchPromotionsAsync(new PromotionSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } promotions have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content folders exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentFolders");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = AbstractTypeFactory<DynamicContentFolderSearchResult>.TryCreateInstance();
                    var result = await LoadFoldersRecursiveAsync(null);
                    searchResult.Results = result;
                    searchResult.TotalCount = result.Count;
                    return (GenericSearchResult<DynamicContentFolder>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content folders have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content items exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentItems");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<DynamicContentItem>)await _contentItemsSearchService.SearchContentItemsAsync(new DynamicContentItemSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content places exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPlaces");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    (GenericSearchResult<DynamicContentPlace>)await _contentPlacesSearchService.SearchContentPlacesAsync(new DynamicContentPlaceSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content places have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content publications exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPublications");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                {
                    var searchResult = await _contentPublicationsSearchService.SearchContentPublicationsAsync(new DynamicContentPublicationSearchCriteria { Skip = skip, Take = take });
                    return (GenericSearchResult<DynamicContentPublication>)searchResult;
                }, (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content publications have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Coupons exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Coupons");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                  (GenericSearchResult<Coupon>)await _couponSearchService.SearchCouponsAsync(new CouponSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } coupons have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Usages exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Usages");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                (GenericSearchResult<PromotionUsage>)await _promotionUsageSearchService.SearchUsagesAsync(new PromotionUsageSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
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

        private async Task<IList<DynamicContentFolder>> LoadFoldersRecursiveAsync(DynamicContentFolder folder)
        {
            var result = new List<DynamicContentFolder>();

            var childrenFolders = (await _folderSearchService.SearchFoldersAsync(new DynamicContentFolderSearchCriteria { FolderId = folder?.Id, Take = int.MaxValue })).Results.ToList();
            foreach (var childFolder in childrenFolders)
            {
                result.Add(childFolder);
                result.AddRange(await LoadFoldersRecursiveAsync(childFolder));
            }

            return result;
        }
    }
}

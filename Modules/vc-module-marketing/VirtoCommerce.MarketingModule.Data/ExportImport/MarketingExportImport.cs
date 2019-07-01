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

        public MarketingExportImport(
            JsonSerializer jsonSerializer, IPromotionSearchService promotionSearchService,
            IDynamicContentSearchService dynamicContentSearchService, IPromotionService promotionService,
            IDynamicContentService dynamicContentService, ICouponService couponService,
            IPromotionUsageService promotionUsageService)
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

                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    await LoadPromotionsAsync(new PromotionSearchCriteria { Skip = skip, Take = take })
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
                    await LoadContentItemsAsync(new DynamicContentItemSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content items have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content places exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPlaces");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    await LoadContentPlacesAsync(new DynamicContentPlaceSearchCriteria { Skip = skip, Take = take })
                , (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content places have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Dynamic content publications exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("DynamicContentPublications");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    await LoadContentPublicationsAsync(new DynamicContentPublicationSearchCriteria { Skip = skip, Take = take }), (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } dynamic content publications have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Coupons exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Coupons");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize, async (skip, take) =>
                    await LoadCouponsAsync(new CouponSearchCriteria { Skip = skip, Take = take }),
                    (processedCount, totalCount) =>
                {
                    progressInfo.Description = $"{ processedCount } of { totalCount } coupons have been exported";
                    progressCallback(progressInfo);
                }, cancellationToken);

                progressInfo.Description = "Usages exporting...";
                progressCallback(progressInfo);

                await writer.WritePropertyNameAsync("Usages");
                await writer.SerializeJsonArrayWithPagingAsync(_jsonSerializer, _batchSize,  async (skip, take) =>
                    await LoadPromotionUsagesAsync(new PromotionUsageSearchCriteria { Skip = skip, Take = take }),
                    (processedCount, totalCount) =>
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
                            await reader.DeserializeJsonArrayWithPagingAsync<Promotion>(_jsonSerializer, _batchSize, SavePromotionsAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } promotions have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);

                        }
                        else if (reader.Value.ToString() == "DynamicContentFolders")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentFolder>(_jsonSerializer, _batchSize, SaveFoldersAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentPlaces")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentPlace>(_jsonSerializer, _batchSize, SavePlacesAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content places have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentItems")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentItem>(_jsonSerializer, _batchSize, SaveContentItemsAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content items have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "DynamicContentPublications")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<DynamicContentPublication>(_jsonSerializer, _batchSize, SavePublicationsAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } dynamic content publications have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Coupons")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<Coupon>(_jsonSerializer, _batchSize, SaveCouponsAsync, processedCount =>
                            {
                                progressInfo.Description = $"{ processedCount } coupons have been imported";
                                progressCallback(progressInfo);
                            }, cancellationToken);
                        }
                        else if (reader.Value.ToString() == "Usages")
                        {
                            await reader.DeserializeJsonArrayWithPagingAsync<PromotionUsage>(_jsonSerializer, _batchSize, SaveUsagesAsync, processedCount =>
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

            var childrenFolders = (await LoadFoldersAsync(new DynamicContentFolderSearchCriteria { FolderId = folder?.Id, Take = int.MaxValue })).Results.ToList();
            foreach (var childFolder in childrenFolders)
            {
                result.Add(childFolder);
                result.AddRange(await LoadFoldersRecursiveAsync(childFolder));
            }

            return result;
        }

        #region Load methods

        protected virtual async Task<GenericSearchResult<Promotion>> LoadPromotionsAsync(PromotionSearchCriteria criteria)
        {
            return await _promotionSearchService.SearchPromotionsAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<DynamicContentFolder>> LoadFoldersAsync(DynamicContentFolderSearchCriteria criteria)
        {
            return await _dynamicContentSearchService.SearchFoldersAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<DynamicContentPlace>> LoadContentPlacesAsync(DynamicContentPlaceSearchCriteria criteria)
        {
            return await _dynamicContentSearchService.SearchContentPlacesAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<DynamicContentItem>> LoadContentItemsAsync(DynamicContentItemSearchCriteria criteria)
        {
            return await _dynamicContentSearchService.SearchContentItemsAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<DynamicContentPublication>> LoadContentPublicationsAsync(DynamicContentPublicationSearchCriteria criteria)
        {
            return await _dynamicContentSearchService.SearchContentPublicationsAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<Coupon>> LoadCouponsAsync(CouponSearchCriteria criteria)
        {
            return await _couponService.SearchCouponsAsync(criteria);
        }

        protected virtual async Task<GenericSearchResult<PromotionUsage>> LoadPromotionUsagesAsync(PromotionUsageSearchCriteria criteria)
        {
            return await _promotionUsageService.SearchUsagesAsync(criteria);
        }

        #endregion Load methods

        #region Save methods

        protected virtual async Task SavePromotionsAsync(IEnumerable<Promotion> promotions)
        {
            await _promotionService.SavePromotionsAsync(promotions.ToArray());
        }

        protected virtual async Task SaveFoldersAsync(IEnumerable<DynamicContentFolder> folders)
        {
            await _dynamicContentService.SaveFoldersAsync(folders.ToArray());
        }

        protected virtual async Task SavePlacesAsync(IEnumerable<DynamicContentPlace> dynamicContentPlaces)
        {
            await _dynamicContentService.SavePlacesAsync(dynamicContentPlaces.ToArray());
        }

        protected virtual async Task SaveContentItemsAsync(IEnumerable<DynamicContentItem> dynamicContentItems)
        {
            await _dynamicContentService.SaveContentItemsAsync(dynamicContentItems.ToArray());
        }

        protected virtual async Task SavePublicationsAsync(IEnumerable<DynamicContentPublication> dynamicContentPublications)
        {
            await _dynamicContentService.SavePublicationsAsync(dynamicContentPublications.ToArray());
        }

        protected virtual async Task SaveCouponsAsync(IEnumerable<Coupon> coupons)
        {
            await _couponService.SaveCouponsAsync(coupons.ToArray());
        }

        protected virtual async Task SaveUsagesAsync(IEnumerable<PromotionUsage> promotionUsages)
        {
            await _promotionUsageService.SaveUsagesAsync(promotionUsages.ToArray());
        }

        #endregion

    }
}

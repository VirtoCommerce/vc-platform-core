using System;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Marketing.Model.DynamicContent.Search;
using VirtoCommerce.Domain.Marketing.Model.Promotions.Search;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.Domain.Marketing.Model;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class MarketingSearchServiceImpl : IDynamicContentSearchService, IPromotionSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly IPromotionService _promotionSrevice;

        public MarketingSearchServiceImpl(Func<IMarketingRepository> repositoryFactory, IDynamicContentService dynamicContentService, IPromotionService promotionSrevice)
        {
            _repositoryFactory = repositoryFactory;
            _dynamicContentService = dynamicContentService;
            _promotionSrevice = promotionSrevice;
        }

        #region IPromotionSearchService Members
        public virtual GenericSearchResult<coreModel.Promotion> SearchPromotions(PromotionSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.Promotion>();
            using (var repository = _repositoryFactory())
            {
                var query = GetPromotionsQuery(repository, criteria);

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<coreModel.Promotion>(x => x.Priority), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var ids = query.Select(x => x.Id)
                               .Skip(criteria.Skip)
                               .Take(criteria.Take).ToArray();
                retVal.Results = _promotionSrevice.GetPromotionsByIds(ids).OrderBy(p => ids.ToList().IndexOf(p.Id)).ToList();
            }
            return retVal;
        }

        protected virtual IQueryable<Model.PromotionEntity> GetPromotionsQuery(IMarketingRepository repository, PromotionSearchCriteria criteria)
        {
            var query = repository.Promotions;

            if (!string.IsNullOrEmpty(criteria.Store))
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => s.StoreId == criteria.Store));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => criteria.StoreIds.Contains(s.StoreId)));
            }

            if (criteria.OnlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsActive && (x.StartDate == null || now >= x.StartDate) && (x.EndDate == null || x.EndDate >= now));
            }
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
            }

            return query;
        }

        #endregion

        #region IDynamicContentSearchService Members
        public GenericSearchResult<coreModel.DynamicContentItem> SearchContentItems(DynamicContentItemSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentItem>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.Items;
                if (!string.IsNullOrEmpty(criteria.FolderId))
                {
                    query = query.Where(x => x.FolderId == criteria.FolderId);
                }
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(q => q.Name.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<coreModel.DynamicContentItem>(x => x.Name), SortDirection = SortDirection.Ascending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var ids = query.Select(x => x.Id)
                               .Skip(criteria.Skip)
                               .Take(criteria.Take).ToArray();
                retVal.Results = _dynamicContentService.GetContentItemsByIds(ids);
            }
            return retVal;
        }

        public GenericSearchResult<coreModel.DynamicContentPlace> SearchContentPlaces(DynamicContentPlaceSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentPlace>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.Places;
                if (!string.IsNullOrEmpty(criteria.FolderId))
                {
                    query = query.Where(x => x.FolderId == criteria.FolderId);
                }
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(q => q.Name.Contains(criteria.Keyword));
                }
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<coreModel.DynamicContentPlace>(x => x.Name), SortDirection = SortDirection.Ascending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();
                var ids = query.Select(x => x.Id)
                               .Skip(criteria.Skip)
                               .Take(criteria.Take).ToArray();
                retVal.Results = _dynamicContentService.GetPlacesByIds(ids);
            }
            return retVal;
        }

        public GenericSearchResult<coreModel.DynamicContentPublication> SearchContentPublications(DynamicContentPublicationSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentPublication>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.PublishingGroups;
                if (!string.IsNullOrEmpty(criteria.Store))
                {
                    query = query.Where(x => x.StoreId == criteria.Store);
                }
                if (criteria.OnlyActive)
                {
                    query = query.Where(x => x.IsActive == true);
                }
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(q => q.Name.Contains(criteria.Keyword));
                }
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<coreModel.DynamicContentPublication>(x => x.Name), SortDirection = SortDirection.Ascending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var ids = query.Select(x => x.Id)
                           .Skip(criteria.Skip)
                           .Take(criteria.Take).ToArray();
                retVal.Results = _dynamicContentService.GetPublicationsByIds(ids);
            }
            return retVal;
        }

        public GenericSearchResult<coreModel.DynamicContentFolder> SearchFolders(DynamicContentFolderSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentFolder>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.Folders.Where(x => x.ParentFolderId == criteria.FolderId);
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(q => q.Name.Contains(criteria.Keyword));
                }
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<coreModel.DynamicContentFolder>(x => x.Name), SortDirection = SortDirection.Ascending } };
                }

                retVal.TotalCount = query.Count();

                var folderIds = query.Select(x => x.Id).ToArray();
                retVal.Results = _dynamicContentService.GetFoldersByIds(folderIds);
            }
            return retVal;
        }
        #endregion

    }
}
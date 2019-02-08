using System;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Domain.Marketing.Model.Promotions.Search;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class PromotionUsageService : ServiceBase, IPromotionUsageService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;

        public PromotionUsageService(Func<IMarketingRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        #region IMarketingUsageService Members

        public virtual GenericSearchResult<PromotionUsage> SearchUsages(PromotionUsageSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            using (var repository = _repositoryFactory())
            {
                var query = GetPromotionUsageQuery(repository, criteria);

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PromotionUsage>(x => x.ModifiedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                var searchResult = new GenericSearchResult<PromotionUsage> { TotalCount = query.Count() };

                var coupons = query.Skip(criteria.Skip).Take(criteria.Take).ToList();
                searchResult.Results = coupons.Select(x => x.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance())).ToList();

                return searchResult;
            }
        }

        public virtual PromotionUsage[] GetByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetMarketingUsagesByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance())).ToArray();
            }
        }

        public virtual void SaveUsages(PromotionUsage[] usages)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existUsageEntities = repository.GetMarketingUsagesByIds(usages.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var usage in usages)
                {
                    var sourceUsageEntity = AbstractTypeFactory<PromotionUsageEntity>.TryCreateInstance();
                    if (sourceUsageEntity != null)
                    {
                        sourceUsageEntity = sourceUsageEntity.FromModel(usage, pkMap);
                        var targetUsageEntity = existUsageEntities.FirstOrDefault(x => x.Id == usage.Id);
                        if (targetUsageEntity != null)
                        {
                            changeTracker.Attach(targetUsageEntity);
                            sourceUsageEntity.Patch(targetUsageEntity);
                        }
                        else
                        {
                            repository.Add(sourceUsageEntity);
                        }
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual void DeleteUsages(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveMarketingUsages(ids);
                CommitChanges(repository);
            }
        }


        protected virtual IQueryable<PromotionUsageEntity> GetPromotionUsageQuery(IMarketingRepository repository, PromotionUsageSearchCriteria criteria)
        {
            var query = repository.PromotionUsages;

            if (!string.IsNullOrEmpty(criteria.PromotionId))
            {
                query = query.Where(x => x.PromotionId == criteria.PromotionId);
            }
            if (!string.IsNullOrEmpty(criteria.CouponCode))
            {
                query = query.Where(x => x.CouponCode == criteria.CouponCode);
            }
            if (!string.IsNullOrEmpty(criteria.ObjectId))
            {
                query = query.Where(x => x.ObjectId == criteria.ObjectId);
            }
            if (!string.IsNullOrEmpty(criteria.ObjectType))
            {
                query = query.Where(x => x.ObjectType == criteria.ObjectType);
            }
            if (!string.IsNullOrWhiteSpace(criteria.UserId))
            {
                query = query.Where(x => x.UserId == criteria.UserId);
            }
            if (!string.IsNullOrWhiteSpace(criteria.UserName))
            {
                query = query.Where(x => x.UserName == criteria.UserName);
            }

            return query;
        }
        #endregion
    }
}

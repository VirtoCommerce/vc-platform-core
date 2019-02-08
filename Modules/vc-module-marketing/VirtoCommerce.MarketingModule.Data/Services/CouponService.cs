using System;
using System.Linq;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class CouponService : ICouponService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        public CouponService(Func<IMarketingRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        #region ICouponService members

        public GenericSearchResult<Coupon> SearchCoupons(CouponSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException("criteria");
            }

            using (var repository = _repositoryFactory())
            {
                var query = repository.Coupons;

                if (!string.IsNullOrEmpty(criteria.PromotionId))
                {
                    query = query.Where(c => c.PromotionId == criteria.PromotionId);
                }
                if (!string.IsNullOrEmpty(criteria.Code))
                {
                    query = query.Where(c => c.Code == criteria.Code);
                }
                if (!criteria.Codes.IsNullOrEmpty())
                {
                    query = query.Where(c => criteria.Codes.Contains(c.Code));
                }

                var sortInfos = criteria.SortInfos;
                //TODO: Sort by TotalUsesCount 
                if (sortInfos.IsNullOrEmpty() || sortInfos.Any(x => x.SortColumn.EqualsInvariant(ReflectionUtility.GetPropertyName<Coupon>(p => p.TotalUsesCount))))
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Coupon>(x => x.Code), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                var searchResult = new GenericSearchResult<Coupon> { TotalCount = query.Count() };

                var ids = query.Select(x => x.Id)
                               .Skip(criteria.Skip)
                               .Take(criteria.Take).ToArray();

                searchResult.Results = GetByIds(ids);
                return searchResult;
            }
        }

        public Coupon[] GetByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetCouponsByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<Coupon>.TryCreateInstance())).ToArray();
            }
        }

        public void SaveCoupons(Coupon[] coupons)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existCouponEntities = repository.GetCouponsByIds(coupons.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var coupon in coupons)
                {
                    var sourceCouponEntity = AbstractTypeFactory<CouponEntity>.TryCreateInstance();
                    if (sourceCouponEntity != null)
                    {
                        sourceCouponEntity = sourceCouponEntity.FromModel(coupon, pkMap);
                        var targetCouponEntity = existCouponEntities.FirstOrDefault(x => x.Id == coupon.Id);
                        if (targetCouponEntity != null)
                        {
                            changeTracker.Attach(targetCouponEntity);
                            sourceCouponEntity.Patch(targetCouponEntity);
                        }
                        else
                        {
                            repository.Add(sourceCouponEntity);
                        }
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public void DeleteCoupons(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveCoupons(ids);
                CommitChanges(repository);
            }
        }

        #endregion

    }
}

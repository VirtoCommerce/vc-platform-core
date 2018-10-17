using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.Repositories;

namespace VirtoCommerce.PricingModule.Data.Services
{
    public class PricingSearchServiceImpl : IPricingSearchService
    {
        private readonly Func<IPricingRepository> _repositoryFactory;
        //private readonly ICatalogSearchService _catalogSearchService;
        private readonly IPricingService _pricingService;
        private readonly Dictionary<string, string> _pricesSortingAliases = new Dictionary<string, string>();

        public PricingSearchServiceImpl(Func<IPricingRepository> repositoryFactory, IPricingService pricingService)
        {
            _repositoryFactory = repositoryFactory;
            //_catalogSearchService = catalogSearchService;
            _pricesSortingAliases["prices"] = ReflectionUtility.GetPropertyName<Price>(x => x.List);
            _pricingService = pricingService;
        }


        #region IPricingSearchService Members

        public virtual Task<PricingSearchResult<Price>> SearchPricesAsync(PricesSearchCriteria criteria)
        {
            return Task.FromResult(new PricingSearchResult<Price>());

            // TODO: uncomment the following implementation when the ICatalogSearchService will become available in CatalogModule

            //var retVal = new PricingSearchResult<Price>();
            //ICollection<CatalogProduct> products = new List<CatalogProduct>();
            //using (var repository = _repositoryFactory())
            //{
            //    var query = repository.Prices;

            //    if (!criteria.PriceListIds.IsNullOrEmpty())
            //    {
            //        query = query.Where(x => criteria.PriceListIds.Contains(x.PricelistId));
            //    }

            //    if (!criteria.ProductIds.IsNullOrEmpty())
            //    {
            //        query = query.Where(x => criteria.ProductIds.Contains(x.ProductId));
            //    }

            //    if (!string.IsNullOrEmpty(criteria.Keyword))
            //    {
            //        var catalogSearchResult = _catalogSearchService.Search(new Domain.Catalog.Model.SearchCriteria { Keyword = criteria.Keyword, Skip = criteria.Skip, Take = criteria.Take, Sort = criteria.Sort.Replace("product.", string.Empty), ResponseGroup = Domain.Catalog.Model.SearchResponseGroup.WithProducts });
            //        var productIds = catalogSearchResult.Products.Select(x => x.Id).ToArray();
            //        query = query.Where(x => productIds.Contains(x.ProductId));
            //        //preserve resulting products for future assignment to prices
            //        products = catalogSearchResult.Products;
            //    }

            //    if (criteria.ModifiedSince.HasValue)
            //    {
            //        query = query.Where(x => x.ModifiedDate >= criteria.ModifiedSince);
            //    }

            //    var sortInfos = criteria.SortInfos.ToArray();
            //    if (sortInfos.IsNullOrEmpty())
            //    {
            //        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Price>(x => x.List) } };
            //    }
            //    //Try to replace sorting columns names
            //    TryTransformSortingInfoColumnNames(_pricesSortingAliases, sortInfos);


            //    query = query.OrderBySortInfos(sortInfos);

            //    if (criteria.GroupByProducts)
            //    {
            //        var groupedQuery = query.GroupBy(x => x.ProductId).OrderBy(x => 1);
            //        retVal.TotalCount = groupedQuery.Count();
            //        query = groupedQuery.Skip(criteria.Skip).Take(criteria.Take).SelectMany(x => x);
            //    }
            //    else
            //    {
            //        retVal.TotalCount = query.Count();
            //        query = query.Skip(criteria.Skip).Take(criteria.Take);
            //    }

            //    var pricesIds = query.Select(x => x.Id).ToList();
            //    retVal.Results = (await _pricingService.GetPricesByIdAsync(pricesIds.ToArray()))
            //                                .OrderBy(x => pricesIds.IndexOf(x.Id))
            //                                .ToList();
            //}
            //return retVal;
        }

        public virtual async Task<PricingSearchResult<Pricelist>> SearchPricelistsAsync(PricelistSearchCriteria criteria)
        {
            var retVal = new PricingSearchResult<Pricelist>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.Pricelists;
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Pricelist>(x => x.Name) } };
                }

                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();
                query = query.Skip(criteria.Skip).Take(criteria.Take);

                var pricelistsIds = query.Select(x => x.Id).ToList();
                retVal.Results = (await _pricingService.GetPricelistsByIdAsync(pricelistsIds.ToArray()))
                                                .OrderBy(x => pricelistsIds.IndexOf(x.Id)).ToList();
            }
            return retVal;
        }



        public virtual async Task<PricingSearchResult<PricelistAssignment>> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria)
        {
            var retVal = new PricingSearchResult<PricelistAssignment>();
            using (var repository = _repositoryFactory())
            {
                var query = repository.PricelistAssignments;

                if (!criteria.PriceListIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.PriceListIds.Contains(x.PricelistId));
                }

                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PricelistAssignment>(x => x.Priority) } };
                }

                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();
                query = query.Skip(criteria.Skip).Take(criteria.Take);

                var pricelistAssignmentsIds = query.Select(x => x.Id).ToList();
                retVal.Results = (await _pricingService.GetPricelistAssignmentsByIdAsync(pricelistAssignmentsIds.ToArray()))
                                                .OrderBy(x => pricelistAssignmentsIds.IndexOf(x.Id))
                                                .ToList();
            }
            return retVal;
        }
        #endregion

        private static void TryTransformSortingInfoColumnNames(IDictionary<string, string> transformationMap, SortInfo[] sortingInfos)
        {
            //Try to replace sorting columns names
            foreach (var sortInfo in sortingInfos)
            {
                if (transformationMap.TryGetValue(sortInfo.SortColumn.ToLowerInvariant(), out var newColumnName))
                {
                    sortInfo.SortColumn = newColumnName;
                }
            }
        }
    }
}


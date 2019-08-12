using System;
using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricingFullPagedDataSource : IPagedDataSource
    {
        private readonly IPricingSearchService _searchService;

        public int PageSize { get; set; } = 50;
        public int CurrentPageNumber { get; }
        public ExportDataQuery DataQuery { get; set; }


        public PricingFullPagedDataSource(IPricingSearchService searchService)
        {
            _searchService = searchService;
        }

        public int GetTotalCount()
        {
            int totalCount = 0;
            var pricelists = _searchService.SearchPricelistsAsync(new PricelistSearchCriteria() { Skip = 0, Take = 0 }).GetAwaiter().GetResult();
            totalCount += pricelists.TotalCount;
            var assignments = _searchService.SearchPricelistAssignmentsAsync(new PricelistAssignmentsSearchCriteria() { Skip = 0, Take = 0 }).GetAwaiter().GetResult();
            totalCount += assignments.TotalCount;
            var prices = _searchService.SearchPricesAsync(new PricesSearchCriteria() { Skip = 0, Take = 0 }).GetAwaiter().GetResult();
            totalCount += prices.TotalCount;
            return totalCount;


        }

        public IEnumerable<ICloneable> FetchNextPage()
        {


            int skip = PageSize * CurrentPageNumber;
            int take = PageSize;

            var pricelists = _searchService.SearchPricelistsAsync(new PricelistSearchCriteria() { Skip = skip, Take = take }).GetAwaiter().GetResult();

            var assignments = _searchService.SearchPricelistAssignmentsAsync(new PricelistAssignmentsSearchCriteria() { Skip = skip, Take = take }).GetAwaiter().GetResult();

            var prices = _searchService.SearchPricesAsync(new PricesSearchCriteria() { Skip = skip, Take = take }).GetAwaiter().GetResult();



        }

        public ViewableSearchResult GetData()
        {
            throw new NotImplementedException();
        }

    }
}

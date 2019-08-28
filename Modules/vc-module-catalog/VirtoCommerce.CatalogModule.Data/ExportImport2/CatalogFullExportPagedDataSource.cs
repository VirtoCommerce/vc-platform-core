using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSource : IPagedDataSource
    {
        private class ExportDataSourceState
        {
            public int TotalCount;
            public IEnumerable<IExportable> Result = Array.Empty<IExportable>();

            public SearchCriteriaBase SearchCriteria { get; set; }
            public Func<ExportDataSourceState, Task> FetchFunc { get; set; }
        }

        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IProductSearchService _productSearchService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;

        public CatalogFullExportPagedDataSource(ICatalogSearchService catalogSearchService, IProductSearchService productSearchService, ICategorySearchService categorySearchService,
                                  IPropertySearchService propertySearchService, IProperyDictionaryItemSearchService propertyDictionarySearchService, CatalogFullExportDataQuery dataQuery)
        {
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
            _propertySearchService = propertySearchService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _catalogSearchService = catalogSearchService;
            DataQuery = dataQuery;
        }

        public int TotalCount { get; set; }
        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        public IEnumerable<IExportable> Items { get; protected set; }

        private readonly List<ExportDataSourceState> _exportDataSourceStates = new List<ExportDataSourceState>();

        private CatalogFullExportDataQuery _dataQuery;

        public CatalogFullExportDataQuery DataQuery
        {
            set
            {
                _dataQuery = value;
                CurrentPageNumber = 0;
                TotalCount = -1;

                _exportDataSourceStates.Add(new ExportDataSourceState
                {
                    SearchCriteria = new CatalogSearchCriteria { ResponseGroup = CatalogResponseGroup.Info.ToString(), CatalogIds = _dataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _catalogSearchService.SearchCatalogsAsync((CatalogSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                });

                _exportDataSourceStates.Add(new ExportDataSourceState
                {
                    SearchCriteria = new CategorySearchCriteria { CatalogIds = _dataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _categorySearchService.SearchCategoriesAsync((CategorySearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                });

                _exportDataSourceStates.Add(new ExportDataSourceState
                {
                    SearchCriteria = new PropertySearchCriteria { CatalogIds = _dataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _propertySearchService.SearchPropertiesAsync((PropertySearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                });

                _exportDataSourceStates.Add(new ExportDataSourceState
                {
                    SearchCriteria = new PropertyDictionaryItemSearchCriteria() { CatalogIds = _dataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _propertyDictionarySearchService.SearchAsync((PropertyDictionaryItemSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                });

                _exportDataSourceStates.Add(new ExportDataSourceState
                {
                    SearchCriteria = new ProductSearchCriteria() { CatalogIds = _dataQuery.CatalogIds, ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString() },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _productSearchService.SearchProductsAsync((ProductSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                });

                CalculateCounts();
            }
        }

        public int GetTotalCount()
        {
            CalculateCounts();
            return TotalCount;
        }

        private void EnsureHaveTotals()
        {
            if (TotalCount < 0)
            {
                CalculateCounts();
            }
        }

        public bool Fetch()
        {
            EnsureHaveTotals();

            var take = _dataQuery.Take ?? PageSize;
            var skip = _dataQuery.Skip ?? CurrentPageNumber * PageSize;
            var taskList = new List<Task>();

            var curStateStartPos = 0; // Starting position of current state

            foreach (var state in _exportDataSourceStates)
            {
                state.Result = Array.Empty<IExportable>();
                var overlap = skip - curStateStartPos; // Positive overlap should fall into skip, negative into take
                state.SearchCriteria.Skip = overlap < 0 ? 0 : overlap;
                state.SearchCriteria.Take = (skip + take > curStateStartPos + state.TotalCount) ? curStateStartPos + state.TotalCount - skip : take + (overlap < 0 ? overlap : 0);

                if (state.SearchCriteria.Take > 0) // Skip running this fetch in case of nothing to take
                {
                    taskList.Add(state.FetchFunc(state));
                }

                curStateStartPos += state.TotalCount;
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult(); // Ensures completing all fetches

            Items = _exportDataSourceStates.SelectMany(x => x.Result);
            CurrentPageNumber++;

            return Items.Any();
        }

        private void CalculateCounts()
        {
            var taskList = new List<Task>();
            foreach (var state in _exportDataSourceStates)
            {
                state.SearchCriteria.Skip = 0;
                state.SearchCriteria.Take = 0;
                taskList.Add(state.FetchFunc(state));
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult(); // Ensures completing all fetches
            TotalCount = _exportDataSourceStates.Sum(x => x.TotalCount);
        }


    }
}

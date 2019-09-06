using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSource : IPagedDataSource
    {
        private readonly ICatalogExportPagedDataSourceFactory _catalogDataSourceFactory;
        private readonly CatalogFullExportDataQuery _dataQuery;
        private readonly object _lock = new object();

        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;
        public int? Skip { get => _dataQuery.Skip; set => _dataQuery.Skip = value; }
        public int? Take { get => _dataQuery.Take; set => _dataQuery.Take = value; }
        public IEnumerable<IExportable> Items { get; protected set; }

        public CatalogFullExportPagedDataSource(ICatalogExportPagedDataSourceFactory catalogDataSourceFactory,
            CatalogFullExportDataQuery dataQuery)
        {
            _catalogDataSourceFactory = catalogDataSourceFactory;
            _dataQuery = dataQuery;

            if (dataQuery == null)
            {
                throw new ArgumentNullException(nameof(dataQuery));
            }
        }

        public bool Fetch()
        {
            var skip = Skip ?? CurrentPageNumber * PageSize;
            var take = Take ?? PageSize;
            Items = DataSources.GetItems(skip, take);
            CurrentPageNumber++;

            return !Items.IsNullOrEmpty();
        }

        public int GetTotalCount() => DataSources.Sum(x => x.GetTotalCount());

        private IEnumerable<IPagedDataSource> _dataSources;
        protected virtual IEnumerable<IPagedDataSource> DataSources
        {
            get
            {
                if(_dataSources == null)
                {
                    lock(_lock)
                    {
                        if (_dataSources == null)
                        { 
                            _dataSources = _catalogDataSourceFactory.GetAllFullExportPagedDataSources(_dataQuery);                            
                        }
                    }                   
                }
                return _dataSources;
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailOptionSearchService : IThumbnailOptionSearchService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailOptionSearchService(Func<IThumbnailRepository> thumbnailRepositoryFactory)
        {
            _thumbnailRepositoryFactory = thumbnailRepositoryFactory;
        }

        public async Task<GenericSearchResult<ThumbnailOption>> SearchAsync(ThumbnailOptionSearchCriteria criteria)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                    sortInfos = new[]
                    {
                        new SortInfo
                        {
                            SortColumn = ReflectionUtility.GetPropertyName<ThumbnailTask>(t => t.CreatedDate), SortDirection = SortDirection.Descending
                        }
                    };

                var query = repository.ThumbnailOptions.OrderBySortInfos(sortInfos);
                var totalCount = await query.CountAsync();

                var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArrayAsync();
                var thumbnailOptions = await repository.GetThumbnailOptionsByIdsAsync(ids);
                var results = thumbnailOptions.Select(t => t.ToModel(AbstractTypeFactory<ThumbnailOption>.TryCreateInstance())).ToArray();

                var retVal = new GenericSearchResult<ThumbnailOption>
                {
                    TotalCount = totalCount,
                    Results = results.AsQueryable().OrderBySortInfos(sortInfos).ToList()
                };

                return retVal;
            }
        }
    }
}

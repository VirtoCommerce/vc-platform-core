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
        public ThumbnailOptionSearchService(Func<IThumbnailRepository> thumbnailRepositoryFactory)
        {
            ThumbnailRepositoryFactory = thumbnailRepositoryFactory;
        }

        protected Func<IThumbnailRepository> ThumbnailRepositoryFactory { get; }

        public virtual async Task<GenericSearchResult<ThumbnailOption>> SearchAsync(ThumbnailOptionSearchCriteria criteria)
        {
            using (var repository = ThumbnailRepositoryFactory())
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

                var results = new ThumbnailOption[0];

                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArrayAsync();
                    var thumbnailOptions = await repository.GetThumbnailOptionsByIdsAsync(ids);
                    results = thumbnailOptions.Select(t => t.ToModel(AbstractTypeFactory<ThumbnailOption>.TryCreateInstance())).ToArray();
                }

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

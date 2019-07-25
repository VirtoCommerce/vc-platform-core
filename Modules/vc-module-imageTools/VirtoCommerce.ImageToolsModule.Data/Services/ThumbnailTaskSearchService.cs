using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailTaskSearchService : IThumbnailTaskSearchService
    {
        private readonly IThumbnailTaskService _thumbnailTaskService;
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailTaskSearchService(Func<IThumbnailRepository> thumbnailRepositoryFactory, IThumbnailTaskService thumbnailTaskService)
        {
            _thumbnailRepositoryFactory = thumbnailRepositoryFactory;
            _thumbnailTaskService = thumbnailTaskService;
        }

        public virtual async Task<ThumbnailTaskSearchResult> SearchAsync(ThumbnailTaskSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ThumbnailTaskSearchResult>.TryCreateInstance();

            using (var repository = _thumbnailRepositoryFactory())
            {
                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await _thumbnailTaskService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }
            }
            return result;
        }

        protected virtual IQueryable<ThumbnailTaskEntity> BuildQuery(IThumbnailRepository repository, ThumbnailTaskSearchCriteria criteria)
        {
            var query = repository.ThumbnailTasks;

            if (!criteria.Keyword.IsNullOrEmpty())
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(ThumbnailTaskSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo {
                        SortColumn = nameof(ThumbnailOptionEntity.ModifiedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }
    }
}

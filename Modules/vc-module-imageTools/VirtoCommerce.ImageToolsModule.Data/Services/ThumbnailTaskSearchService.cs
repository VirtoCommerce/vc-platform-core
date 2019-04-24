using System;
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
        public ThumbnailTaskSearchService(Func<IThumbnailRepository> thumbnailThumbnailRepositoryFactoryFactory, IThumbnailTaskService thumbnailTaskService)
        {
            ThumbnailRepositoryFactory = thumbnailThumbnailRepositoryFactoryFactory;
            ThumbnailTaskService = thumbnailTaskService;
        }

        protected IThumbnailTaskService ThumbnailTaskService { get; }
        protected Func<IThumbnailRepository> ThumbnailRepositoryFactory { get; }

        public virtual async Task<GenericSearchResult<ThumbnailTask>> SearchAsync(ThumbnailTaskSearchCriteria criteria)
        {
            var result = new GenericSearchResult<ThumbnailTask>();

            using (var repository = ThumbnailRepositoryFactory())
            {
                var query = GetTasksQuery(repository, criteria);

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                        new SortInfo
                        {
                            SortColumn = ReflectionUtility.GetPropertyName<ThumbnailTask>(t => t.CreatedDate), SortDirection = SortDirection.Descending
                        }
                    };
                }
                query = query.OrderBySortInfos(sortInfos);
                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    result.Results = (await ThumbnailTaskService.GetByIdsAsync(ids)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                }
            }
            return result;
        }

        protected virtual IQueryable<ThumbnailTaskEntity> GetTasksQuery(IThumbnailRepository repository,
            ThumbnailTaskSearchCriteria criteria)
        {
            var query = repository.ThumbnailTasks;

            if (!criteria.Keyword.IsNullOrEmpty())
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            return query;
        }
    }
}

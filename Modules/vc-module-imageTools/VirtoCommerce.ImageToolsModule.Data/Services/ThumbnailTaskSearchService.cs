using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailTaskSearchService : ServiceBase, IThumbnailTaskSearchService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailTaskSearchService(Func<IThumbnailRepository> thumbnailThumbnailRepositoryFactoryFactory)
        {
            _thumbnailRepositoryFactory = thumbnailThumbnailRepositoryFactoryFactory;
        }

        public async Task<ThumbnailTaskSearchResult> SearchAsync(ThumbnailTaskSearchCriteria criteria)
        {
            using (var repository = _thumbnailRepositoryFactory())
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
                var totalCount = query.Count();

                var ids = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArray();
                var thumbnailTasks = await repository.GetThumbnailTasksByIdsAsync(ids);
                var results = thumbnailTasks.Select(t => t.ToModel(AbstractTypeFactory<ThumbnailTask>.TryCreateInstance())).ToArray();

                var retVal = new ThumbnailTaskSearchResult
                {
                    TotalCount = totalCount,
                    Results = results.AsQueryable().OrderBySortInfos(sortInfos).ToList()
                };

                return retVal;
            }
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

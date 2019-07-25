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
    public class ThumbnailOptionSearchService : IThumbnailOptionSearchService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactor;
        private readonly IThumbnailOptionService _optionsService;

        public ThumbnailOptionSearchService(Func<IThumbnailRepository> thumbnailRepositoryFactor, IThumbnailOptionService optionsService)
        {
            _thumbnailRepositoryFactor = thumbnailRepositoryFactor;
            _optionsService = optionsService;
        }

        public virtual async Task<ThumbnailOptionSearchResult> SearchAsync(ThumbnailOptionSearchCriteria criteria)
        {
            using (var repository = _thumbnailRepositoryFactor())
            {
                var result = AbstractTypeFactory<ThumbnailOptionSearchResult>.TryCreateInstance();

                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await _optionsService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }
                return result;
            }
        }

        protected virtual IQueryable<ThumbnailOptionEntity> BuildQuery(IThumbnailRepository repository, ThumbnailOptionSearchCriteria criteria)
        {
            var query = repository.ThumbnailOptions;           
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(ThumbnailOptionSearchCriteria criteria)
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

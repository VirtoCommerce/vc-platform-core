using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Services.Indexed;
using VirtoCommerce.CustomerModule.Data.Caching;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    public class MemberSearchService : IMemberSearchService
    {
        private readonly Func<IMemberRepository> _repositoryFactory;
        private readonly IMemberService _memberService;
        private readonly IIndexedMemberSearchService _indexedSearchService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public MemberSearchService(
            Func<IMemberRepository> repositoryFactory
            , IMemberService memberService
            , IIndexedMemberSearchService indexedSearchService
            , IPlatformMemoryCache platformMemoryCache
            )
        {
            _repositoryFactory = repositoryFactory;
            _memberService = memberService;
            _indexedSearchService = indexedSearchService;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IMemberSearchService Members

        public virtual Task<MemberSearchResult> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            var result = !string.IsNullOrEmpty(criteria?.Keyword)
                ? IndexedSearchMembersAsync(criteria)
                : RegularSearchMembersAsync(criteria);

            return result;
        }
        #endregion


        protected virtual Task<MemberSearchResult> IndexedSearchMembersAsync(MembersSearchCriteria criteria)
        {
            return _indexedSearchService.SearchMembersAsync(criteria);
        }

        /// <summary>
        /// Search members in database by given criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual async Task<MemberSearchResult> RegularSearchMembersAsync(MembersSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchMembersAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerSearchCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var result = AbstractTypeFactory<MemberSearchResult>.TryCreateInstance();

                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(repository, criteria);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                            .Select(x => x.Id)
                                            .Skip(criteria.Skip).Take(criteria.Take)
                                            .ToArrayAsync();

                        result.Results = (await _memberService.GetByIdsAsync(ids, criteria.ResponseGroup)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                    return result;
                }
            });
        }

        protected virtual IQueryable<MemberEntity> BuildQuery(IMemberRepository repository, MembersSearchCriteria criteria)
        {
            var query = repository.Members;

            if (!criteria.MemberTypes.IsNullOrEmpty())
            {
                query = query.Where(m => criteria.MemberTypes.Contains(m.MemberType));
            }

            if (!criteria.Groups.IsNullOrEmpty())
            {
                query = query.Where(m => m.Groups.Any(g => criteria.Groups.Contains(g.Group)));
            }

            if (criteria.MemberId != null)
            {
                //TODO: DeepSearch in specified member
                query = query.Where(m => m.MemberRelations.Any(r => r.AncestorId == criteria.MemberId));
            }
            else if (!criteria.DeepSearch)
            {
                query = query.Where(m => !m.MemberRelations.Any());
            }

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(m => m.Name.Contains(criteria.Keyword) || m.Emails.Any(e => e.Address.Contains(criteria.Keyword)));               
            }
          
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(MembersSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] {
                            new SortInfo { SortColumn = nameof(Member.MemberType), SortDirection = SortDirection.Descending },
                            new SortInfo { SortColumn = nameof(Member.Name), SortDirection = SortDirection.Ascending },
                        };
            }
            return sortInfos;
        }
      
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Caching;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    public class MemberSearchServiceBase
    {
        public MemberSearchServiceBase(Func<IMemberRepository> repositoryFactory, IMemberService memberService, IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            MemberService = memberService;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<IMemberRepository> RepositoryFactory { get; }
        protected IMemberService MemberService { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        #region IMemberSearchService Members
        /// <summary>
        /// Search members in database by given criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<GenericSearchResult<Member>> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchMembersAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerSearchCacheRegion.CreateChangeToken());
                using (var repository = RepositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var result = new GenericSearchResult<Member>();

                    var query = LinqKit.Extensions.AsExpandable(repository.Members);

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

                    //Get extra predicates (where clause)
                    var predicate = GetQueryPredicate(criteria);
                    query = query.Where(LinqKit.Extensions.Expand(predicate));

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] {
                            new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Member>(m => m.MemberType), SortDirection = SortDirection.Descending },
                            new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Member>(m => m.Name), SortDirection = SortDirection.Ascending },
                        };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var memberIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await MemberService.GetByIdsAsync(memberIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                    }
                    return result;
                }
            });
        }
        #endregion

        /// <summary>
        /// Used to define extra where clause for members search
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual Expression<Func<MemberEntity, bool>> GetQueryPredicate(MembersSearchCriteria criteria)
        {
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                var predicate = PredicateBuilder.False<MemberEntity>();
                predicate = predicate.Or(m => m.Name.Contains(criteria.Keyword) || m.Emails.Any(e => e.Address.Contains(criteria.Keyword)));
                //Should use Expand() to all predicates to prevent EF error
                //http://stackoverflow.com/questions/2947820/c-sharp-predicatebuilder-entities-the-parameter-f-was-not-bound-in-the-specif?rq=1
                return LinqKit.Extensions.Expand(predicate);
            }
            return PredicateBuilder.True<MemberEntity>();
        }
    }
}

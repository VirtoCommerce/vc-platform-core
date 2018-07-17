using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    public class MemberSearchServiceBase : IMemberSearchService
    {
        private readonly Func<IMemberRepository> _repositoryFactory;
        private readonly IMemberService _memberService;

        public MemberSearchServiceBase(Func<IMemberRepository> repositoryFactory, IMemberService memberService)
        {
            _repositoryFactory = repositoryFactory;
            _memberService = memberService;
        }

        #region IMemberSearchService Members
        /// <summary>
        /// Search members in database by given criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<GenericSearchResult<Member>> SearchMembersAsync(MembersSearchCriteria criteria)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

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

                var totalCount = query.Count();
                var memberIds = query.Select(m => m.Id).Skip(criteria.Skip).Take(criteria.Take).ToList();
                var members = await _memberService.GetByIdsAsync(memberIds.ToArray(), criteria.ResponseGroup, criteria.MemberTypes);

                var result = new GenericSearchResult<Member>
                {
                    TotalCount = totalCount,
                    Results = members.OrderBy(m => memberIds.IndexOf(m.Id)).ToList(),
                };

                return result;
            }
        }
        #endregion

        /// <summary>
        /// Used to define extra where clause for members search
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual Expression<Func<MemberDataEntity, bool>> GetQueryPredicate(MembersSearchCriteria criteria)
        {
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                var predicate = PredicateBuilder.False<MemberDataEntity>();
                predicate = predicate.Or(m => m.Name.Contains(criteria.Keyword) || m.Emails.Any(e => e.Address.Contains(criteria.Keyword)));
                //Should use Expand() to all predicates to prevent EF error
                //http://stackoverflow.com/questions/2947820/c-sharp-predicatebuilder-entities-the-parameter-f-was-not-bound-in-the-specif?rq=1
                return LinqKit.Extensions.Expand(predicate);
            }
            return PredicateBuilder.True<MemberDataEntity>();
        }
    }
}

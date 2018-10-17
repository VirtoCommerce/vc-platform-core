using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    public class CommerceMembersSearchServiceImpl : MemberSearchServiceBase
    {
        private readonly Func<IMemberRepository> _repositoryFactory;
        private readonly IMemberService _memberService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CommerceMembersSearchServiceImpl(Func<IMemberRepository> repositoryFactory, IMemberService memberService, IPlatformMemoryCache platformMemoryCache) : base(repositoryFactory, memberService, platformMemoryCache)
        {
        }

        [SuppressMessage("ReSharper", "TryCastAlwaysSucceeds")]
        protected override Expression<Func<MemberEntity, bool>> GetQueryPredicate(MembersSearchCriteria criteria)
        {
            var retVal = base.GetQueryPredicate(criteria);

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                //where x or (y1 or y2)
                var predicate = PredicateBuilder.False<MemberEntity>();
                //search in special properties
                // do NOT use explicit conversion (also called direct or unsafe) cast i.e. (T(x)). EF doesn't support that
                predicate = predicate.Or(x => x is ContactEntity && (x as ContactEntity).FullName.Contains(criteria.Keyword));
                predicate = predicate.Or(x => x is EmployeeEntity && (x as EmployeeEntity).FullName.Contains(criteria.Keyword));
                //Should use Expand() to all predicates to prevent EF error
                //http://stackoverflow.com/questions/2947820/c-sharp-predicatebuilder-entities-the-parameter-f-was-not-bound-in-the-specif?rq=1
                retVal = LinqKit.Extensions.Expand(retVal.Or(LinqKit.Extensions.Expand(predicate)));
            }

            return retVal;
        }
    }
}

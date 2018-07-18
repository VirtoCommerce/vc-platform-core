using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    /// <summary>
    /// Members service support CRUD and search for Contact, Organization, Vendor and Employee member types
    /// </summary>
    public class CommerceMembersServiceImpl : MemberServiceBase
    {
        //private readonly ISecurityService _securityService;
        public CommerceMembersServiceImpl(Func<ICustomerRepository> repositoryFactory, IEventPublisher eventPublisher
            , IDynamicPropertyService dynamicPropertyService, ISeoService seoService, IPlatformMemoryCache platformMemoryCache)
            : base(repositoryFactory, eventPublisher, dynamicPropertyService, seoService, platformMemoryCache)
        {
        //    _securityService = securityService;
        }


        #region IMemberService Members

        public override async Task<Member[]> GetByIdsAsync(string[] memberIds, string responseGroup = null, string[] memberTypes = null)
        {
            var retVal = await base.GetByIdsAsync(memberIds, responseGroup, memberTypes);
            //TODO
            //Parallel.ForEach(retVal, new ParallelOptions { MaxDegreeOfParallelism = 10 }, member =>
            //{
            //    //Load security accounts for members which support them 
            //    var hasSecurityAccounts = member as IHasSecurityAccounts;
            //    if (hasSecurityAccounts != null)
            //    {
            //        //Load all security accounts associated with this contact
            //        var result = Task.Run(() => _securityService.SearchUsersAsync(new UserSearchRequest { MemberId = member.Id, TakeCount = int.MaxValue })).Result;
            //        hasSecurityAccounts.SecurityAccounts.AddRange(result.Users);
            //    }
            //});
            return retVal;
        }
        #endregion

        //TODO 
        //[SuppressMessage("ReSharper", "TryCastAlwaysSucceeds")]
        //protected override Expression<Func<MemberDataEntity, bool>> GetQueryPredicate(MembersSearchCriteria criteria)
        //{
        //    var retVal = base.GetQueryPredicate(criteria);

        //    if (!string.IsNullOrEmpty(criteria.Keyword))
        //    {
        //        //where x or (y1 or y2)
        //        var predicate = PredicateBuilder.False<MemberDataEntity>();
        //        //search in special properties
        //        // do NOT use explicit conversion (also called direct or unsafe) cast i.e. (T(x)). EF doesn't support that
        //        predicate = predicate.Or(x => x is ContactDataEntity && (x as ContactDataEntity).FullName.Contains(criteria.Keyword));
        //        predicate = predicate.Or(x => x is EmployeeDataEntity && (x as EmployeeDataEntity).FullName.Contains(criteria.Keyword));
        //        //Should use Expand() to all predicates to prevent EF error
        //        //http://stackoverflow.com/questions/2947820/c-sharp-predicatebuilder-entities-the-parameter-f-was-not-bound-in-the-specif?rq=1
        //        retVal = LinqKit.Extensions.Expand(retVal.Or(LinqKit.Extensions.Expand(predicate)));
        //    }

        //    return retVal;
        //}
    }
}

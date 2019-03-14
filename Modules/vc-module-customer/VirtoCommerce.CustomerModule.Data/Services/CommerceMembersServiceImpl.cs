using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    /// <summary>
    /// Members service support CRUD and search for Contact, Organization, Vendor and Employee member types
    /// </summary>
    public class CommerceMembersServiceImpl : MemberServiceBase
    {
        public CommerceMembersServiceImpl(Func<ICustomerRepository> repositoryFactory, IEventPublisher eventPublisher
            , IDynamicPropertyService dynamicPropertyService, ISeoService seoService, IPlatformMemoryCache platformMemoryCache, IUserSearchService userSearchService)
            : base(repositoryFactory, eventPublisher, dynamicPropertyService, seoService, platformMemoryCache)
        {
            UserSearchService = userSearchService;
        }

        protected IUserSearchService UserSearchService { get; }

        #region IMemberService Members

        public override async Task<Member[]> GetByIdsAsync(string[] memberIds, string responseGroup = null, string[] memberTypes = null)
        {
            var result = await base.GetByIdsAsync(memberIds, responseGroup, memberTypes);
            var memberRespGroup = EnumUtility.SafeParse(responseGroup, MemberResponseGroup.Full);
            //Load member security accounts by separate request
            if (memberRespGroup.HasFlag(MemberResponseGroup.WithSecurityAccounts))
            {
                var hasSecurityAccountMembers = result.OfType<IHasSecurityAccounts>();
                if (hasSecurityAccountMembers.Any())
                {
                    var usersSearchResult = await UserSearchService.SearchUsersAsync(new UserSearchCriteria { MemberIds = hasSecurityAccountMembers.Select(x => x.Id).ToList(), Take = int.MaxValue });
                    foreach (var hasAccountMember in hasSecurityAccountMembers)
                    {
                        hasAccountMember.SecurityAccounts = usersSearchResult.Results.Where(x => x.MemberId.EqualsInvariant(hasAccountMember.Id)).ToList();
                    }
                }
            }

            return result;
        }
        #endregion


    }
}

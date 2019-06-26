using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
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
        private readonly IUserSearchService _userSearchService;

        public CommerceMembersServiceImpl(Func<ICustomerRepository> repositoryFactory, IEventPublisher eventPublisher
            , IDynamicPropertyService dynamicPropertyService, IPlatformMemoryCache platformMemoryCache, IUserSearchService userSearchService
            , IDynamicPropertyMetaInfoService dynamicPropertyMetaInfoService)
            : base(repositoryFactory, eventPublisher, dynamicPropertyService, platformMemoryCache, dynamicPropertyMetaInfoService)
        {
            _userSearchService = userSearchService;
        }

        #region IMemberService Members

        public override async Task<Member[]> GetByIdsAsync(string[] memberIds, string responseGroup = null, string[] memberTypes = null)
        {
            var result = await base.GetByIdsAsync(memberIds, responseGroup, memberTypes);
            var memberRespGroup = EnumUtility.SafeParseFlags(responseGroup, MemberResponseGroup.Full);
            //Load member security accounts by separate request
            if (memberRespGroup.HasFlag(MemberResponseGroup.WithSecurityAccounts))
            {
                var hasSecurityAccountMembers = result.OfType<IHasSecurityAccounts>().ToArray();
                if (hasSecurityAccountMembers.Any())
                {
                    var usersSearchResult = await _userSearchService.SearchUsersAsync(new UserSearchCriteria { MemberIds = hasSecurityAccountMembers.Select(x => x.Id).ToList(), Take = int.MaxValue });
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

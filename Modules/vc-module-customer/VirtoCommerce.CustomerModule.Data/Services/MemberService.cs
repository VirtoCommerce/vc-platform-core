using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CustomerModule.Core.Events;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Caching;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerModule.Data.Services
{
    /// <summary>
    /// Abstract base class for all derived custom members services used IMemberRepository for persistent
    /// </summary>
    public class MemberService : IMemberService
    {
        private readonly Func<IMemberRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IUserSearchService _userSearchService;

        public MemberService(
            Func<IMemberRepository> repositoryFactory
            , IUserSearchService userSearchService
            , IEventPublisher eventPublisher
            , IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
            _userSearchService = userSearchService;
        }

        #region IMemberService Members

        /// <summary>
        /// Return members by requested ids can be override for load extra data for resulting members
        /// </summary>
        /// <param name="memberIds"></param>
        /// <param name="responseGroup"></param>
        /// <param name="memberTypes"></param>
        /// <returns></returns>
        public virtual async Task<Member[]> GetByIdsAsync(string[] memberIds, string responseGroup = null, string[] memberTypes = null)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", memberIds), responseGroup, memberTypes == null ? null : string.Join("-", memberTypes));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var retVal = new List<Member>();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    //There is loading for all corresponding members conceptual model entities types
                    //query performance when TPT inheritance used it is too slow, for improve performance we are passing concrete member types in to the repository
                    var memberTypeInfos = AbstractTypeFactory<Member>.AllTypeInfos.Where(t => t.MappedType != null);
                    if (memberTypes != null)
                    {
                        memberTypeInfos = memberTypeInfos.Where(x => memberTypes.Any(mt => x.IsAssignableTo(mt)));
                    }
                    memberTypes = memberTypeInfos.Select(t => t.MappedType.AssemblyQualifiedName).Distinct().ToArray();

                    var dataMembers = await repository.GetMembersByIdsAsync(memberIds, responseGroup, memberTypes);
                    foreach (var dataMember in dataMembers)
                    {
                        var member = AbstractTypeFactory<Member>.TryCreateInstance(dataMember.MemberType);
                        if (member != null)
                        {
                            dataMember.ToModel(member);

                            member.ReduceDetails(responseGroup);

                            retVal.Add(member);
                            cacheEntry.AddExpirationToken(CustomerCacheRegion.CreateChangeToken(member));
                        }
                    }
                }
                var memberRespGroup = EnumUtility.SafeParseFlags(responseGroup, MemberResponseGroup.Full);
                //Load member security accounts by separate request
                if (memberRespGroup.HasFlag(MemberResponseGroup.WithSecurityAccounts))
                {
                    var hasSecurityAccountMembers = retVal.OfType<IHasSecurityAccounts>().ToArray();
                    if (hasSecurityAccountMembers.Any())
                    {
                        var usersSearchResult = await _userSearchService.SearchUsersAsync(new UserSearchCriteria { MemberIds = hasSecurityAccountMembers.Select(x => x.Id).ToList(), Take = int.MaxValue });
                        foreach (var hasAccountMember in hasSecurityAccountMembers)
                        {
                            hasAccountMember.SecurityAccounts = usersSearchResult.Results.Where(x => x.MemberId.EqualsInvariant(hasAccountMember.Id)).ToList();
                        }
                    }
                }
                return retVal.ToArray();
            });
        }

        public virtual async Task<Member> GetByIdAsync(string memberId, string responseGroup = null, string memberType = null)
        {
            var members = await GetByIdsAsync(new[] { memberId }, responseGroup, new[] { memberType });
            return members.FirstOrDefault();
        }

        /// <summary>
        /// Create or update members in database
        /// </summary>
        /// <param name="members"></param>
        public virtual async Task SaveChangesAsync(Member[] members)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Member>>();

            using (var repository = _repositoryFactory())
            {
                var existingMemberEntities = await repository.GetMembersByIdsAsync(members.Where(m => !m.IsTransient()).Select(m => m.Id).ToArray());

                foreach (var member in members)
                {
                    var memberEntityType = AbstractTypeFactory<Member>.AllTypeInfos.Where(t => t.MappedType != null && t.IsAssignableTo(member.MemberType)).Select(t => t.MappedType).FirstOrDefault();
                    if (memberEntityType != null)
                    {
                        var dataSourceMember = AbstractTypeFactory<MemberEntity>.TryCreateInstance(memberEntityType.Name);
                        if (dataSourceMember != null)
                        {
                            dataSourceMember.FromModel(member, pkMap);

                            var dataTargetMember = existingMemberEntities.FirstOrDefault(m => m.Id == member.Id);
                            if (dataTargetMember != null)
                            {
                                changedEntries.Add(new GenericChangedEntry<Member>(member, dataTargetMember.ToModel(AbstractTypeFactory<Member>.TryCreateInstance(member.MemberType)), EntryState.Modified));
                                dataSourceMember.Patch(dataTargetMember);
                            }
                            else
                            {
                                repository.Add(dataSourceMember);
                                changedEntries.Add(new GenericChangedEntry<Member>(member, EntryState.Added));
                            }
                        }
                    }
                }
                //Raise domain events
                await _eventPublisher.Publish(new MemberChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                await _eventPublisher.Publish(new MemberChangedEvent(changedEntries));
            }

            ClearCache(members);
        }

        public virtual async Task DeleteAsync(string[] ids, string[] memberTypes = null)
        {
            using (var repository = _repositoryFactory())
            {
                var members = await GetByIdsAsync(ids, null, memberTypes);
                if (!members.IsNullOrEmpty())
                {
                    var changedEntries = members.Select(x => new GenericChangedEntry<Member>(x, EntryState.Deleted)).ToArray();
                    await _eventPublisher.Publish(new MemberChangingEvent(changedEntries));

                    await repository.RemoveMembersByIdsAsync(members.Select(m => m.Id).ToArray());
                    await repository.UnitOfWork.CommitAsync();
                    await _eventPublisher.Publish(new MemberChangedEvent(changedEntries));
                }

                ClearCache(members);
            }
        }

        protected virtual void ClearCache(IEnumerable<Member> members)
        {
            CustomerSearchCacheRegion.ExpireRegion();

            foreach (var member in members.Where(x => !x.IsTransient()))
            {
                CustomerCacheRegion.ExpireMemberById(member.Id);
            }
        }
        #endregion
    }
}

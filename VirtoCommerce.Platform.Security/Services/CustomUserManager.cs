using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Events;
using VirtoCommerce.Platform.Security.Caching;

namespace VirtoCommerce.Platform.Security.Services
{
    public class CustomUserManager : AspNetUserManager<ApplicationUser>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEventPublisher _eventPublisher;

        public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher,
                                 IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                 ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services,
                                 ILogger<UserManager<ApplicationUser>> logger, RoleManager<Role> roleManager, IMemoryCache memoryCache, IEventPublisher eventPublisher)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _memoryCache = memoryCache;
            _roleManager = roleManager;
            _eventPublisher = eventPublisher;
        }

        public override async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByLoginAsync", loginProvider, providerKey);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByLoginAsync(loginProvider, providerKey);
                if (user != null)
                {
                    await LoadUserRolesAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }
                return user;
            }, cacheNullValue: false);

            return result;
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByEmailAsync", email);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByEmailAsync(email);
                if (user != null)
                {
                    await LoadUserRolesAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }
                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", userName);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByNameAsync(userName);
                if (user != null)
                {
                    await LoadUserRolesAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }
                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var user = await base.FindByIdAsync(userId);
                if (user != null)
                {
                    await LoadUserRolesAsync(user);
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeTokenForUser(user));
                }
                return user;
            }, cacheNullValue: false);
            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var changedEntries = new List<GenericChangedEntry<ApplicationUser>>
            {
                new GenericChangedEntry<ApplicationUser>(user, EntryState.Deleted)
            };
            await _eventPublisher.Publish(new UserChangingEvent(changedEntries));
            var result = await base.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _eventPublisher.Publish(new UserChangedEvent(changedEntries));
                SecurityCacheRegion.ExpireUser(user);
            }
            return result;
        }
        public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            var oldUser = await FindByIdAsync(user.Id);
            var changedEntries = new List<GenericChangedEntry<ApplicationUser>>
            {
                new GenericChangedEntry<ApplicationUser>(user, oldUser, EntryState.Modified)
            };
            await _eventPublisher.Publish(new UserChangingEvent(changedEntries));
            var result = await base.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _eventPublisher.Publish(new UserChangedEvent(changedEntries));
                if (user.Roles != null)
                {
                    var targetRoles = (await GetRolesAsync(user));
                    var sourceRoles = user.Roles.Select(x => x.Name);
                    //Add
                    foreach (var newRole in sourceRoles.Except(targetRoles))
                    {
                        await AddToRoleAsync(user, newRole);
                    }
                    //Remove
                    foreach (var removeRole in targetRoles.Except(sourceRoles))
                    {
                        await RemoveFromRoleAsync(user, removeRole);
                    }
                }
                SecurityCacheRegion.ExpireUser(user);
            }
            return result;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            var changedEntries = new List<GenericChangedEntry<ApplicationUser>>
            {
                new GenericChangedEntry<ApplicationUser>(user, EntryState.Added)
            };
            await _eventPublisher.Publish(new UserChangingEvent(changedEntries));
            var result = await base.CreateAsync(user);
            if (result.Succeeded)
            {
                await _eventPublisher.Publish(new UserChangedEvent(changedEntries));
                if (user.Roles != null)
                {
                    //Add
                    foreach (var newRole in user.Roles)
                    {
                        await AddToRoleAsync(user, newRole.Name);
                    }
                }
                SecurityCacheRegion.ExpireUser(user);
            }
            return result;
        }

        protected virtual async Task LoadUserRolesAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Roles = new List<Role>();
            foreach (var roleName in await base.GetRolesAsync(user))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    user.Roles.Add(role);
                }
            }
        }
    }
}

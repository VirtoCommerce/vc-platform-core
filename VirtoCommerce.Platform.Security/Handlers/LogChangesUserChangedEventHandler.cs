using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Events;

namespace VirtoCommerce.Platform.Security.Handlers
{
    public class LogChangesUserChangedEventHandler : IEventHandler<UserChangedEvent>, IEventHandler<UserLoginEvent>,
                                                     IEventHandler<UserLogoutEvent>, IEventHandler<UserPasswordChangedEvent>,
                                                     IEventHandler<UserResetPasswordEvent>
    {
        private readonly IChangeLogService _changeLogService;
        public LogChangesUserChangedEventHandler(IChangeLogService changeLogService)
        {
            _changeLogService = changeLogService;
        }

        public virtual Task Handle(UserChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    SaveOperationLog(changedEntry.NewEntry.Id, "Created", EntryState.Added);
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    var changes = DetectAccountChanges(changedEntry.NewEntry, changedEntry.OldEntry);
                    foreach (var key in changes.Keys)
                    {
                        SaveOperationLog(changedEntry.NewEntry.Id, string.Format(key, string.Join(", ", changes[key].ToArray())), EntryState.Modified);
                    }
                }
            }
            
            return Task.CompletedTask;
        }

        public virtual Task Handle(UserLoginEvent message)
        {
            return Task.CompletedTask;
        }

        public virtual Task Handle(UserLogoutEvent message)
        {
            return Task.CompletedTask;
        }

        public virtual Task Handle(UserPasswordChangedEvent message)
        {
            SaveOperationLog(message.UserId, "Password changed", EntryState.Modified);
            return Task.CompletedTask;
        }

        public virtual Task Handle(UserResetPasswordEvent message)
        {
            SaveOperationLog(message.UserId, "Password resets", EntryState.Modified);
            return Task.CompletedTask;
        }

        protected virtual ListDictionary<string, string> DetectAccountChanges(ApplicationUser newUser, ApplicationUser oldUser)
        {
            //Log changes
            var result = new ListDictionary<string, string>();
            if (newUser.UserName != oldUser.UserName)
            {
                result.Add("Changes: {0}", $"user name: {oldUser.UserName} -> {newUser.UserName}");
            }
            if (newUser.UserType != oldUser.UserType)
            {
                result.Add("Changes: {0}", $"user type: {oldUser.UserType} -> {newUser.UserType}");
            }

            //todo add after the implementation
            //if (newUser.UserState != oldUser.UserState)
            //{
            //    result.Add("Changes: {0}", $"account state: {oldUser.UserState} -> {newUser.UserState}");
            //}

            if (newUser.IsAdministrator != oldUser.IsAdministrator)
            {
                result.Add("Changes: {0}", $"root: {oldUser.IsAdministrator} -> {newUser.IsAdministrator}");
            }

            //todo add after the implementation
            //if (!newUser.ApiAccounts.IsNullOrEmpty())
            //{
            //    var apiAccountComparer = AnonymousComparer.Create((ApiAccount x) => $"{x.ApiAccountType}-{x.SecretKey}");
            //    newUser.ApiAccounts.CompareTo(oldUser.ApiAccounts ?? Array.Empty<ApiAccount>(), apiAccountComparer, (state, sourceItem, targetItem) =>
            //    {
            //        if (state == EntryState.Added)
            //        {
            //            result.Add("Activated Api Key(s) [{0}] ", $"{sourceItem.Name} ({sourceItem.ApiAccountType})");
            //        }
            //        else if (state == EntryState.Deleted)
            //        {
            //            result.Add("Deactivated Api Key(s) [{0}]</value>", $"{sourceItem.Name} ({sourceItem.ApiAccountType})");
            //        }
            //    }
            //    );
            //}
            if (!newUser.Roles.IsNullOrEmpty())
            {
                newUser.Roles.CompareTo(oldUser.Roles ?? Array.Empty<Role>(), EqualityComparer<Role>.Default, (state, sourceItem, targetItem) =>
                {
                    if (state == EntryState.Added)
                    {
                        result.Add("Added role(s) [{0}]", $"{sourceItem?.Name}");
                    }
                    else if (state == EntryState.Deleted)
                    {
                        result.Add("Removed role(s) [{0}]", $"{sourceItem?.Name}");
                    }
                });
            }

            return result;
        }

        protected virtual void SaveOperationLog(string objectId, string detail, EntryState entryState)
        {
            var operation = new OperationLog
            {
                ObjectId = objectId,
                ObjectType = typeof(ApplicationUser).Name,
                OperationType = entryState,
                Detail = detail
            };
            _changeLogService.SaveChanges(operation);
        }
    }
}

using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Handlers
{
    //TODO
    ///// <summary>
    ///// Send welcome registration email notification when storefront user registered
    ///// </summary>
    //public class RegistrationEmailMemberChangedEventHandler : IEventHandler<MemberChangedEvent>
    //{
    //    private readonly IStoreService _storeService;
    //    private readonly ISecurityService _securityService;
    //    private readonly INotificationManager _notificationManager;

    //    public RegistrationEmailMemberChangedEventHandler(IStoreService storeService, ISecurityService securityService, INotificationManager notificationManager)
    //    {
    //        _storeService = storeService;
    //        _securityService = securityService;
    //        _notificationManager = notificationManager;
    //    }

    //    public virtual async Task Handle(MemberChangedEvent message)
    //    {
    //        var newContacts = message.ChangedEntries
    //            .Where(x => x.EntryState == EntryState.Added)
    //            .Select(x => x.NewEntry)
    //            .OfType<Contact>()
    //            .ToArray();

    //        if (!newContacts.IsNullOrEmpty())
    //        {
    //            var usersSearchResult = await _securityService.SearchUsersAsync(new UserSearchRequest { MemberIds = newContacts.Select(x => x.Id).ToArray(), TakeCount = int.MaxValue });
    //            var storefrontAccounts = usersSearchResult.Users
    //                .Where(x => x.UserType.EqualsInvariant(AccountType.Customer.ToString()))
    //                .ToArray();

    //            if (!storefrontAccounts.IsNullOrEmpty())
    //            {
    //                foreach (var contact in newContacts)
    //                {
    //                    var account = storefrontAccounts.FirstOrDefault(x => x.MemberId == contact.Id);
    //                    if (account != null)
    //                    {
    //                        var store = _storeService.GetById(account.StoreId);
    //                        var notification = _notificationManager.GetNewNotification<RegistrationEmailNotification>(account.StoreId, "Store", string.IsNullOrEmpty(contact.DefaultLanguage) ? store.DefaultLanguage : contact.DefaultLanguage);
    //                        notification.FirstName = contact.FirstName;
    //                        notification.LastName = contact.LastName;
    //                        notification.Login = account.UserName;
    //                        notification.Sender = store.Email;
    //                        notification.Recipient = account.Email;
    //                        _notificationManager.ScheduleSendNotification(notification);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}

using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseSignedEvent : DomainEvent
    {
        public LicenseSignedEvent(License license, string clientIpAddress, bool isActivated)
        {
            License = license;
            ClientIpAddress = clientIpAddress;
            IsActivated = isActivated;
        }

        public License License { get; set; }
        public string ClientIpAddress { get; set; }
        public bool IsActivated { get; set; }
    }
}

using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CustomerModule.Core.Model
{
    public class Contact : Member, IHasSecurityAccounts
    {
        public string Salutation { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }
        public string DefaultLanguage { get; set; }
        public string TimeZone { get; set; }
        public IList<string> Organizations { get; set; }

        public string TaxPayerId { get; set; }
        public string PreferredDelivery { get; set; }
        public string PreferredCommunication { get; set; }
        public string PhotoUrl { get; set; }

        #region IHasSecurityAccounts Members

        /// <summary>
        /// All security accounts associated with this contact
        /// </summary>
        public ICollection<ApplicationUser> SecurityAccounts { get; } = new List<ApplicationUser>();

        #endregion
    }
}

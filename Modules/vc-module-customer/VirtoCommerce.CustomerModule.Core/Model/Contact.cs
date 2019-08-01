using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
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

        public override string ObjectType => typeof(Contact).FullName;

        #region IHasSecurityAccounts Members

        /// <summary>
        /// All security accounts associated with this contact
        /// </summary>
        public ICollection<ApplicationUser> SecurityAccounts { get; set; } = new List<ApplicationUser>();

        #endregion

        public override void ReduceDetails(string responseGroup)
        {
            base.ReduceDetails(responseGroup);
            //Reduce details according to response group
            var memberResponseGroup = EnumUtility.SafeParseFlags(responseGroup, MemberResponseGroup.Full);

            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithSecurityAccounts))
            {
                SecurityAccounts = null;
            }
        }
    }
}

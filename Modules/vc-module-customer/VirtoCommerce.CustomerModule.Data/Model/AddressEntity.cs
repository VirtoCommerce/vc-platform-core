using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using Address = VirtoCommerce.CustomerModule.Core.Model.Address;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class AddressEntity : AuditableEntity
    {
        [StringLength(2048)]
        public string Name { get; set; }

        [StringLength(128)]
        public string FirstName { get; set; }

        [StringLength(128)]
        public string LastName { get; set; }

        [Required]
        [StringLength(128)]
        public string Line1 { get; set; }

        [StringLength(128)]
        public string Line2 { get; set; }

        [Required]
        [StringLength(128)]
        public string City { get; set; }

        [Required]
        [StringLength(64)]
        public string CountryCode { get; set; }

        [StringLength(128)]
        public string StateProvince { get; set; }

        [Required]
        [StringLength(128)]
        public string CountryName { get; set; }

        [Required]
        [StringLength(32)]
        public string PostalCode { get; set; }

        [StringLength(128)]
        public string RegionId { get; set; }

        [StringLength(128)]
        public string RegionName { get; set; }

        [StringLength(64)]
        public string Type { get; set; }

        [StringLength(64)]
        public string DaytimePhoneNumber { get; set; }

        [StringLength(64)]
        public string EveningPhoneNumber { get; set; }

        [StringLength(64)]
        public string FaxNumber { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(128)]
        public string Organization { get; set; }

        #region Navigation Properties

        public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {Line1} {Line2}, {City}, {StateProvince} {PostalCode} {CountryName}";
        }

        #endregion

        public virtual Address ToModel(Address address)
        {
            address.CountryCode = CountryCode;
            address.CountryName = CountryName;
            address.PostalCode = PostalCode;
            address.RegionId = RegionId;
            address.RegionName = RegionName;
            address.City = City;
            address.Name = Name;
            address.Email = Email;
            address.FirstName = FirstName;
            address.LastName = LastName;
            address.Line1 = Line1;
            address.Line2 = Line2;
            address.Key = Id;
            address.Phone = DaytimePhoneNumber;
            address.AddressType = EnumUtility.SafeParseFlags(Type, AddressType.BillingAndShipping);
            return address;
        }

        public virtual AddressEntity FromModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            CountryCode = address.CountryCode;
            CountryName = address.CountryName;
            PostalCode = address.PostalCode;
            RegionId = address.RegionId;
            RegionName = address.RegionName;
            City = address.City;
            Name = address.Name;
            Email = address.Email;
            FirstName = address.FirstName;
            LastName = address.LastName;
            Line1 = address.Line1;
            Line2 = address.Line2;
            Id = address.Key;
            DaytimePhoneNumber = address.Phone;
            Type = address.AddressType.ToString();

            return this;
        }

        public virtual void Patch(AddressEntity target)
        {
            target.CountryCode = CountryCode;
            target.CountryName = CountryName;
            target.DaytimePhoneNumber = DaytimePhoneNumber;
            target.EveningPhoneNumber = EveningPhoneNumber;
            target.FaxNumber = FaxNumber;
            target.PostalCode = PostalCode;
            target.RegionId = RegionId;
            target.RegionName = RegionName;
            target.StateProvince = StateProvince;
            target.Type = Type;
            target.City = City;
            target.Name = Name;
            target.Email = Email;
            target.FirstName = FirstName;
            target.LastName = LastName;
            target.Line1 = Line1;
            target.Line2 = Line2;
            target.Organization = Organization;
        }
    }
}

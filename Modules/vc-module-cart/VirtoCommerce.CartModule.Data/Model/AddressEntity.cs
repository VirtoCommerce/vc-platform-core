using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class AddressEntity : Entity
    {
        [StringLength(2048)]
        public string Name { get; set; }

        [StringLength(32)]
        public string AddressType { get; set; }

        [StringLength(64)]
        public string Organization { get; set; }

        [StringLength(3)]
        public string CountryCode { get; set; }

        [Required]
        [StringLength(64)]
        public string CountryName { get; set; }

        [Required]
        [StringLength(128)]
        public string City { get; set; }

        [StringLength(64)]
        public string PostalCode { get; set; }

        [StringLength(2048)]
        public string Line1 { get; set; }

        [StringLength(2048)]
        public string Line2 { get; set; }

        [StringLength(128)]
        public string RegionId { get; set; }

        [StringLength(128)]
        public string RegionName { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        [StringLength(64)]
        public string Phone { get; set; }

        [StringLength(254)]
        public string Email { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }

        public virtual Address ToModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            //TODO
            //address.Key = Id;
            address.Name = Name;
            address.AddressType = EnumUtility.SafeParseFlags(AddressType, CoreModule.Core.Common.AddressType.BillingAndShipping);
            address.City = City;
            address.CountryCode = CountryCode;
            address.CountryName = CountryName;
            address.Email = Email;
            address.FirstName = FirstName;
            address.LastName = LastName;
            address.Line1 = Line1;
            address.Line2 = Line2;
            //address.MiddleName = Middle
            address.Organization = Organization;
            address.Phone = Phone;
            address.PostalCode = PostalCode;
            address.RegionId = RegionId;
            address.RegionName = RegionName;
            //address.Zip =

            return address;
        }

        public virtual AddressEntity FromModel(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            //TODO
            //Key = address.Id;
            Name = address.Name;
            AddressType = address.AddressType.ToString();
            City = address.City;
            CountryCode = address.CountryCode;
            CountryName = address.CountryName;
            Email = address.Email;
            FirstName = address.FirstName;
            LastName = address.LastName;
            Line1 = address.Line1;
            Line2 = address.Line2;
            //MiddleName = 
            Organization = address.Organization;
            Phone = address.Phone;
            PostalCode = address.PostalCode;
            RegionId = address.RegionId;
            RegionName = address.RegionName;
            //Zip =

            return this;
        }

        public virtual void Patch(AddressEntity target)
        {
            target.Name = Name;
            target.City = City;
            target.CountryCode = CountryCode;
            target.CountryName = CountryName;
            target.Phone = Phone;
            target.PostalCode = PostalCode;
            target.RegionId = RegionId;
            target.RegionName = RegionName;
            target.AddressType = AddressType;
            target.City = City;
            target.Email = Email;
            target.FirstName = FirstName;
            target.LastName = LastName;
            target.Line1 = Line1;
            target.Line2 = Line2;
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            //For transient addresses need to compare two objects as value object (by content)
            if (!result && IsTransient() && obj is AddressEntity otherAddressEntity)
            {
                var domainAddress = ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
                var otherAddress = otherAddressEntity.ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
                result = domainAddress.Equals(otherAddress);
            }
            return result;
        }

        public override int GetHashCode()
        {
            if (IsTransient())
            {
                //need to convert to domain address model to allow use ValueObject.GetHashCode
                var domainAddress = ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
                return domainAddress.GetHashCode();
            }
            return base.GetHashCode();
        }
    }
}

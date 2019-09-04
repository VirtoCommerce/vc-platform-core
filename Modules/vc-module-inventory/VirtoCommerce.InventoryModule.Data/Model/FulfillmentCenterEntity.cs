using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Data.Model
{
    public class FulfillmentCenterEntity : AuditableEntity, IHasOuterId
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(1024)]
        public string Line1 { get; set; }

        [StringLength(1024)]
        public string Line2 { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [StringLength(64)]
        public string CountryCode { get; set; }

        [StringLength(128)]
        public string StateProvince { get; set; }

        [StringLength(128)]
        public string CountryName { get; set; }

        [StringLength(32)]
        public string PostalCode { get; set; }

        [StringLength(128)]
        public string RegionId { get; set; }

        [StringLength(128)]
        public string RegionName { get; set; }

        [StringLength(64)]
        public string DaytimePhoneNumber { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(128)]
        public string Organization { get; set; }

        [StringLength(64)]
        public string GeoLocation { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        public virtual FulfillmentCenter ToModel(FulfillmentCenter center)
        {
            center.Id = Id;
            center.CreatedBy = CreatedBy;
            center.CreatedDate = CreatedDate;
            center.ModifiedBy = ModifiedBy;
            center.ModifiedDate = ModifiedDate;
            center.OuterId = OuterId;

            center.Address = AbstractTypeFactory<Address>.TryCreateInstance();

            center.Address.City = City;
            center.Address.CountryCode = CountryCode;
            center.Address.CountryName = CountryName;
            center.Address.Phone = DaytimePhoneNumber;
            center.Address.Line1 = Line1;
            center.Address.Line2 = Line2;
            center.Address.PostalCode = PostalCode;
            center.Address.RegionName = StateProvince;
            center.Address.RegionId = RegionId;
            center.Address.Email = Email;

            center.Description = Description;
            center.Name = Name;
            center.GeoLocation = GeoLocation;

            return center;
        }

        public virtual FulfillmentCenterEntity FromModel(FulfillmentCenter center, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(center, this);

            Id = center.Id;
            CreatedBy = center.CreatedBy;
            CreatedDate = center.CreatedDate;
            ModifiedBy = center.ModifiedBy;
            ModifiedDate = center.ModifiedDate;
            OuterId = center.OuterId;

            if (center.Address != null)
            {
                City = center.Address.City;
                CountryCode = center.Address.CountryCode;
                CountryName = center.Address.CountryName;
                DaytimePhoneNumber = center.Address.Phone;
                Line1 = center.Address.Line1;
                Line2 = center.Address.Line2;
                PostalCode = center.Address.PostalCode;
                StateProvince = center.Address.RegionName;
                RegionId = center.Address.RegionId;
                Email = center.Address.Email;
            }

            Description = center.Description;
            Name = center.Name;
            GeoLocation = center.GeoLocation;

            return this;
        }

        public virtual void Patch(FulfillmentCenterEntity target)
        {
            target.City = City;
            target.CountryCode = CountryCode;
            target.CountryName = CountryName;
            target.DaytimePhoneNumber = DaytimePhoneNumber;
            target.Line1 = Line1;
            target.Line2 = Line2;
            target.PostalCode = PostalCode;
            target.StateProvince = StateProvince;
            target.RegionId = RegionId;
            target.Email = Email;
            target.Description = Description;
            target.Name = Name;
            target.GeoLocation = GeoLocation;
        }
    }
}

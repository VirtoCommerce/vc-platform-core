using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Model
{
    public class LicenseEntity : AuditableEntity
    {
        [Required]
        [StringLength(64)]
        public string Type { get; set; }

        [Required]
        [StringLength(256)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(256)]
        public string CustomerEmail { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [StringLength(64)]
        public string ActivationCode { get; set; }

        public virtual License ToModel(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            license.Id = Id;
            license.CreatedDate = CreatedDate;
            license.CreatedBy = CreatedBy;
            license.ModifiedDate = ModifiedDate;
            license.ModifiedBy = ModifiedBy;

            license.Type = Type;
            license.CustomerName = CustomerName;
            license.CustomerEmail = CustomerEmail;
            license.ExpirationDate = ExpirationDate;
            license.ActivationCode = ActivationCode;

            return license;
        }

        public virtual LicenseEntity FromModel(License license, PrimaryKeyResolvingMap pkMap)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            pkMap.AddPair(license, this);

            Id = license.Id;
            CreatedDate = license.CreatedDate;
            CreatedBy = license.CreatedBy;
            ModifiedDate = license.ModifiedDate;
            ModifiedBy = license.ModifiedBy;

            Type = license.Type;
            CustomerName = license.CustomerName;
            CustomerEmail = license.CustomerEmail;
            ExpirationDate = license.ExpirationDate;
            ActivationCode = license.ActivationCode;

            return this;
        }

        public virtual void Patch(LicenseEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ActivationCode = ActivationCode;
            target.CustomerEmail = CustomerEmail;
            target.CustomerName = CustomerName;
            target.ExpirationDate = ExpirationDate;
            target.Type = Type;
        }
    }
}

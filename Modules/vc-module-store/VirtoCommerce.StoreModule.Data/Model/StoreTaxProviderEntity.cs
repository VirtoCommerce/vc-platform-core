using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Domain.Tax.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreTaxProviderEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }


        #region Navigation Properties

        public string StoreId { get; set; }

        public StoreEntity Store { get; set; }

        #endregion


        public virtual TaxProvider ToModel(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException(nameof(taxProvider));

            taxProvider.IsActive = IsActive;
            taxProvider.Code = Code;
            taxProvider.Description = Description;
            taxProvider.LogoUrl = LogoUrl;
            taxProvider.Name = Name;
            taxProvider.Priority = Priority;

            return taxProvider;
        }

        public virtual StoreTaxProviderEntity FromModel(TaxProvider taxProvider, PrimaryKeyResolvingMap pkMap)
        {
            if (taxProvider == null)
                throw new ArgumentNullException(nameof(taxProvider));

            pkMap.AddPair(taxProvider, this);

            IsActive = taxProvider.IsActive;
            Code = taxProvider.Code;
            Description = taxProvider.Description;
            LogoUrl = taxProvider.LogoUrl;
            Name = taxProvider.Name;
            Priority = taxProvider.Priority;

            return this;
        }

        public virtual void Patch(StoreTaxProviderEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.Code = Code;
            target.Description = Description;
            target.LogoUrl = LogoUrl;
            target.Name = Name;
            target.Priority = Priority;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.TaxModule.Data.Model
{
    public class StoreTaxProviderEntity : Entity
    {
        [Required]
        [StringLength(128)]
        public string Code { get; set; }

        public int Priority { get; set; }

        [Required]
        [StringLength(128)]
        public string TypeName { get; set; }

        [StringLength(2048)]
        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }

        #endregion

        public virtual TaxProvider ToModel(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException(nameof(taxProvider));

            taxProvider.Id = Id;
            taxProvider.IsActive = IsActive;
            taxProvider.Code = Code;
            taxProvider.LogoUrl = LogoUrl;
            taxProvider.Priority = Priority;
            taxProvider.StoreId = StoreId;
            return taxProvider;
        }

        public virtual StoreTaxProviderEntity FromModel(TaxProvider taxProvider, PrimaryKeyResolvingMap pkMap)
        {
            if (taxProvider == null)
                throw new ArgumentNullException(nameof(taxProvider));

            pkMap.AddPair(taxProvider, this);
            Id = taxProvider.Id;
            IsActive = taxProvider.IsActive;
            Code = taxProvider.Code;
            LogoUrl = taxProvider.LogoUrl;
            Priority = taxProvider.Priority;
            StoreId = taxProvider.StoreId;
            TypeName = taxProvider.TypeName;
            return this;
        }

        public virtual void Patch(StoreTaxProviderEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.IsActive = IsActive;
            target.LogoUrl = LogoUrl;
            target.Priority = Priority;
        }
    }
}

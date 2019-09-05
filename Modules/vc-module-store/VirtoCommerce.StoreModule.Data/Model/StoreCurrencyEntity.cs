using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Data.Model
{
    public class StoreCurrencyEntity : Entity
    {
        [Required]
        [StringLength(32)]
        public string CurrencyCode { get; set; }

        #region Navigation Properties

        public string StoreId { get; set; }
        public StoreEntity Store { get; set; }

        #endregion
    }
}

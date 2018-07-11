using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Core.Shipping
{
    public abstract class ShippingMethod : Entity, IHasSettings
    {
        /// <summary>
        /// Method identity property (System name)
        /// </summary>
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public string TaxType { get; set; }


        #region IHasSettings Members

        public ICollection<ObjectSettingEntry> Settings { get; set; }

        #endregion

        public abstract IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context);

        public virtual string TypeName => GetType().Name;
    }
}

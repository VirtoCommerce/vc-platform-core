using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ShippingModule.Core.Model
{
    public abstract class ShippingMethod : Entity, IHasSettings, ICloneable
    {
        public ShippingMethod(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Method identity property (System name)
        /// </summary>
        public string Code { get; set; }

        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public string TaxType { get; set; }

        public string StoreId { get; set; }

        #region IHasSettings Members

        public ICollection<ObjectSettingEntry> Settings { get; set; }

        #endregion

        public abstract IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context);

        public virtual string TypeName => GetType().Name;

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as ShippingMethod;

            result.Settings = Settings?.Select(x => x.Clone()).OfType<ObjectSettingEntry>().ToList();

            return result;
        }

        #endregion
    }
}

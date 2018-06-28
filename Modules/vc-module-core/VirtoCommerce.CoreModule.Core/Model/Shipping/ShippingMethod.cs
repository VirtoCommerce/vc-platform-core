using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Core.Model.Shipping
{
    public abstract class ShippingMethod : Entity, IHaveSettings
    {
        private ShippingMethod()
        {
            Id = Guid.NewGuid().ToString("N");
            IsActive = true;
        }

        public ShippingMethod(string code)
            :this()
		{
			Code = code;
		}

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


		#region IHaveSettings Members

		public ICollection<SettingEntry> Settings { get; set; }

		#endregion

		public abstract IEnumerable<ShippingRate> CalculateRates(IEvaluationContext context);

        public virtual string TypeName => GetType().Name;
    }
}

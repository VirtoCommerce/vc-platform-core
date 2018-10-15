using System;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
	public class Price : AuditableEntity, ICloneable
	{
		public string PricelistId { get; set; }
        public Pricelist Pricelist { get; set; }
		public string Currency { get; set; }
		public string ProductId { get; set; }
		public decimal? Sale { get; set; }
		public decimal List { get; set; }
		public int MinQuantity { get; set; }

		public decimal EffectiveValue
		{
			get
			{
				return Sale ?? List;
			}
		}

		#region ICloneable Members

		public object Clone()
		{
            var jObject = JObject.FromObject(this);
            var result = jObject.ToObject(this.GetType());
            return result;
        }

		#endregion
	}
}

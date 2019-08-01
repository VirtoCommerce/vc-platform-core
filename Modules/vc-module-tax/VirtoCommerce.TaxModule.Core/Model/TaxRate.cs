using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaxModule.Core.Model
{
    public class TaxRate : ValueObject
    {
        /// <summary>
        /// The absolute amount of tax rate
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// The percent tax rate
        /// </summary>
        public decimal PercentRate { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        public string Currency { get; set; }

        public TaxLine Line { get; set; }
        [JsonIgnore]
        public TaxProvider TaxProvider { get; set; }
        public string TaxProviderCode { get; set; }

        public IList<TaxDetail> TaxDetails { get; set; } = new List<TaxDetail>();
    }
}

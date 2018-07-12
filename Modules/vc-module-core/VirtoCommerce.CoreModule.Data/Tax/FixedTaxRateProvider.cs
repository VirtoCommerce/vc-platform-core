using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Data.Tax
{
    public class FixedTaxRateProvider : TaxProvider
    {
        private decimal Rate
        {
            get
            {
                return Settings.GetSettingValue(ModuleConstants.Settings.General.FixedTaxRateProviderRate.Name, 0m);
            }
        }

        public override IEnumerable<TaxRate> CalculateRates(IEvaluationContext context)
        {
            var taxEvalContext = context as TaxEvaluationContext;
            if (taxEvalContext == null)
            {
                throw new NullReferenceException("taxEvalContext");
            }

            var retVal = new List<TaxRate>();

            foreach (var line in taxEvalContext.Lines)
            {
                var rate = AbstractTypeFactory<TaxRate>.TryCreateInstance();

                rate.Rate = line.Amount * Rate * 0.01m;
                rate.Currency = taxEvalContext.Currency;
                rate.TaxProvider = this;
                rate.Line = line;

                retVal.Add(rate);
            }

            return retVal;
        }
    }
}

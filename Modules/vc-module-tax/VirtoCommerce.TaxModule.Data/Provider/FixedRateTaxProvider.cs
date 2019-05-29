using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaxModule.Core;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.TaxModule.Data.Provider
{
    public class FixedRateTaxProvider : TaxProvider
    {
        public FixedRateTaxProvider()
        {
            Code = "FixedRate";
        }
        private decimal Rate
        {
            get
            {
                return Settings.GetSettingValue(ModuleConstants.Settings.FixedTaxProviderSettings.FixedTaxRateProviderRate.Name, 0m);
            }
        }

        public override IEnumerable<TaxRate> CalculateRates(IEvaluationContext context)
        {
            var taxEvalContext = context as TaxEvaluationContext;
            if (taxEvalContext == null)
            {
                throw new NullReferenceException(nameof(context));
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

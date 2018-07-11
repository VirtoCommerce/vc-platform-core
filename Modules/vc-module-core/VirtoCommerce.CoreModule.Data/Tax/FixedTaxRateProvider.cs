using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Model.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Data.Tax
{
    public class FixedTaxRateProvider : TaxProvider
    {
        public FixedTaxRateProvider()
        {
        }

        public FixedTaxRateProvider(params SettingEntry[] settings)
            : this()
        {
            Settings = settings;
        }

        private decimal Rate
        {
            get
            {
                decimal retVal = 0;
                var settingRate = Settings.FirstOrDefault(x => x.Name == ModuleConstants.Settings.General.FixedTaxRateProviderRate.Name);
                if (settingRate != null)
                {
                    retVal = Decimal.Parse(settingRate.Value, CultureInfo.InvariantCulture);
                }
                return retVal;
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

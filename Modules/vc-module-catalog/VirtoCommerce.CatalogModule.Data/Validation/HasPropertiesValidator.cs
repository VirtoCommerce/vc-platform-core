using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Validation
{
    /// <summary>
    /// Custom validator for IHasProperties object - validates property values by attached PropertyValidationRule
    /// </summary>
    public class HasPropertiesValidator : AbstractValidator<IHasProperties>
    {
        private readonly Func<PropertyValidationRule, PropertyValueValidator> _propertyValidatorFactory;

        public HasPropertiesValidator(Func<PropertyValidationRule, PropertyValueValidator> propertyValidatorFactory)
        {
            _propertyValidatorFactory = propertyValidatorFactory;
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<IHasProperties> context, CancellationToken cancellation = default(CancellationToken))
        {
            var validationResults = new List<ValidationResult>();
            if (!context.InstanceToValidate.Properties.IsNullOrEmpty())
            {
                var propertyValues = context.InstanceToValidate.Properties.Where(x => x.Values != null).SelectMany(pv => pv.Values).ToArray();
                if (!propertyValues.IsNullOrEmpty())
                {
                    foreach (var propertyValue in propertyValues)
                    {
                        var rules = propertyValue?.Property?.ValidationRules;
                        if (rules != null)
                        {
                            foreach (var rule in rules)
                            {
                                var ruleValidator = _propertyValidatorFactory(rule);
                                var validationResult = await ruleValidator.ValidateAsync(propertyValue, cancellation);
                                validationResults.Add(validationResult);
                            }
                        }
                    }
                }
            }

            var errors = validationResults.SelectMany(x => x.Errors);
            return new ValidationResult(errors);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CatalogModule.Core.Model;
using PropertyValidationRule = VirtoCommerce.CatalogModule.Core.Model.PropertyValidationRule;

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

        public override ValidationResult Validate(ValidationContext<IHasProperties> context)
        {
            var validationResults = new List<ValidationResult>();

            foreach (var property in context.InstanceToValidate?.Properties ?? Enumerable.Empty<Property>())
            {
                foreach (var rule in property?.ValidationRules ?? Enumerable.Empty<PropertyValidationRule>())
                {
                    var ruleValidator = _propertyValidatorFactory(rule);
                    foreach (var value in property?.Values ?? Enumerable.Empty<PropertyValue>())
                    {
                        var validationResult = ruleValidator.Validate(value);
                        validationResults.Add(validationResult);
                    }
                }
            }
            var errors = validationResults.SelectMany(x => x.Errors);
            return new ValidationResult(errors);
        }
    }
}

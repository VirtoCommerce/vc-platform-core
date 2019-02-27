using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.Common;
using PropertyValidationRule = VirtoCommerce.Domain.Catalog.Model.PropertyValidationRule;

namespace VirtoCommerce.CatalogModule.Data.Services.Validation
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
            var propertyValues = context.InstanceToValidate.PropertyValues;
            if (propertyValues != null)
            {
                foreach (var propertyValue in propertyValues)
                {
                    var rules = propertyValue?.Property?.ValidationRules;
                    if (rules != null)
                    {
                        foreach (var rule in rules)
                        {
                            var ruleValidator = _propertyValidatorFactory(rule);
                            var validationResult = ruleValidator.Validate(propertyValue);
                            validationResults.Add(validationResult);
                        }
                    }
                }
            }

            var errors = validationResults.SelectMany(x => x.Errors);
            return new ValidationResult(errors);
        }
    }
}

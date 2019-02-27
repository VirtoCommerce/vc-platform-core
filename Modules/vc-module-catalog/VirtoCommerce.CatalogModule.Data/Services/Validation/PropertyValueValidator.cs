using FluentValidation;
using System;
using VirtoCommerce.Domain.Catalog.Model;

namespace VirtoCommerce.CatalogModule.Data.Services.Validation
{
    /// <summary>
    /// Custom validator for PropertyValue object, dynamically creates validation rules by passed PropertyValidationRule
    /// </summary>
    public class PropertyValueValidator : AbstractValidator<PropertyValue>
    {
        public PropertyValueValidator(PropertyValidationRule rule)
        {
            AttachValidators(rule);
        }

        private void AttachValidators(PropertyValidationRule rule)
        {
            bool notNullOrEmptyPredicate(PropertyValue x) => x.Value != null && !string.IsNullOrEmpty(x.Value.ToString());

            if (rule.CharCountMax.HasValue)
            {
                RuleFor(s => s.Value.ToString())
                .MaximumLength(rule.CharCountMax.Value).WithName(rule.Property.Name)
                .When(notNullOrEmptyPredicate);
            }

            if (rule.CharCountMin.HasValue)
            {
                RuleFor(s => s.Value.ToString())
                .MinimumLength(rule.CharCountMin.Value).WithName(rule.Property.Name)
                .When(notNullOrEmptyPredicate);
            }

            if (!string.IsNullOrEmpty(rule.RegExp))
            {
                RuleFor(s => s.Value.ToString())
                  .Matches(rule.RegExp).WithName(rule.Property.Name)
                  .When(notNullOrEmptyPredicate);
            }
        }    
    }
}
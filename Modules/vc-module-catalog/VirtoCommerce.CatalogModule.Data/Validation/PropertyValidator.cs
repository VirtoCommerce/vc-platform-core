using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.Validation
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        private static readonly char[] _illegalCodeChars = { '$', '+', ';', '=', '%', '{', '}', '[', ']', '|', '\\', '/', '@', '~', '!', '^', '*', '&', '(', ')', ':', '<', '>' };
        public PropertyValidator()
        {

            //RuleFor(property => property.CatalogId).NotNull().NotEmpty();
            RuleFor(property => property.Name).NotNull().NotEmpty().WithMessage(x => $"Name is null or empty").MaximumLength(128);
            //TODO:
            //RuleFor(product => product.Code).NotNull().NotEmpty().MaximumLength(64).DependentRules(d => d.RuleFor(product => product.Code).Must(x => x.IndexOfAny(_illegalCodeChars) < 0).WithMessage("product code contains illegal chars"));
        }
    }
}

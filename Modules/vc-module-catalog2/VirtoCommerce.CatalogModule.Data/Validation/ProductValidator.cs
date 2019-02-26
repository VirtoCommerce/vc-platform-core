using FluentValidation;
using VirtoCommerce.CatalogModule.Core2.Model;

namespace VirtoCommerce.CatalogModule.Data2.Validation
{
    public class ProductValidator : AbstractValidator<CatalogProduct>
    {
        private static readonly char[] _illegalCodeChars = { '$', '+', ';', '=', '%', '{', '}', '[', ']', '|', '\\', '/', '@', '~', '!', '^', '*', '&', '(', ')', ':', '<', '>' };
        public ProductValidator()
        {

            RuleFor(product => product.CatalogId).NotNull().NotEmpty();
            RuleFor(product => product.Name).NotNull().NotEmpty().WithMessage(x => $"Name is null or empty.").MaximumLength(1024);
            //TODO:
            //RuleFor(product => product.Code).NotNull().NotEmpty().MaximumLength(64).DependentRules(d => d.RuleFor(product => product.Code).Must(x => x.IndexOfAny(_illegalCodeChars) < 0).WithMessage("product code contains illegal chars"));
        }
    }
}

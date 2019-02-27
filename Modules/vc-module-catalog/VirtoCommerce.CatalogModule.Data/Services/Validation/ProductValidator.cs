using FluentValidation;
using VirtoCommerce.Domain.Catalog.Model;

namespace VirtoCommerce.CatalogModule.Data.Services.Validation
{
    public class ProductValidator : AbstractValidator<CatalogProduct>
    {
        private static readonly char[] _illegalCodeChars = { '$', '+', ';', '=', '%', '{', '}', '[', ']', '|', '\\', '/', '@', '~', '!', '^', '*', '&', '(', ')', ':', '<', '>' };
        public ProductValidator()
        {

            RuleFor(product => product.CatalogId).NotNull().NotEmpty();
            RuleFor(product => product.Name).NotNull().WithMessage(x => $"Name is null. Code: {x.Code}").NotEmpty().WithMessage(x => $"Name is empty. Code: {x.Code}").MaximumLength(1024);
            RuleFor(product => product.Code).NotNull().NotEmpty().MaximumLength(64).DependentRules(() => RuleFor(product => product.Code).Must(x => x.IndexOfAny(_illegalCodeChars) < 0).WithMessage("product code contains illegal chars"));

        }
    }
}

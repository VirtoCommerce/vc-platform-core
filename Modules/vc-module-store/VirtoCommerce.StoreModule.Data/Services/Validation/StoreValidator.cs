using FluentValidation;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Data.Services.Validation
{
    public class StoreValidator : AbstractValidator<Store>
    {
        public StoreValidator()
        {
            RuleFor(store => store.Id).NotEmpty().Matches(@"^[a-zA-Z0-9_\-]*$");
        }
    }
}

using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Handlers
{
    public class CartChangeEventHandler : IEventHandler<CartChangeEvent>
    {
        public Task Handle(CartChangeEvent message)
        {
            return Task.CompletedTask;
        }
    }
}

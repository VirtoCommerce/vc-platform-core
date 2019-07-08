using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Handlers
{
    public class CartChangedEventHandler : IEventHandler<CartChangedEvent>, IEventHandler<CartChangeEvent>
    {
        public virtual Task Handle(CartChangedEvent message)
        {
            return Task.CompletedTask;
        }

        public Task Handle(CartChangeEvent message)
        {
            return Task.CompletedTask;
        }
    }
}

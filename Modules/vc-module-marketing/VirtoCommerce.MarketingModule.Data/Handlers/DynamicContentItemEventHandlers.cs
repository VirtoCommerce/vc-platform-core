using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Data.Handlers
{
    public class DynamicContentItemEventHandlers : IEventHandler<DynamicContentItemChangedEvent>
    {
        public Task Handle(DynamicContentItemChangedEvent message)
        {
            return Task.CompletedTask;
        }
    }
}

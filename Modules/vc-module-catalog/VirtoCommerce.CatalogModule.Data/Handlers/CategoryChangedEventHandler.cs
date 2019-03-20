using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Data.Handlers
{
    public class CategoryChangedEventHandler : IEventHandler<CategoryChangedEvent>, IEventHandler<CategoryChangingEvent>
    {
        private readonly ISeoService _seoService;

        public CategoryChangedEventHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task Handle(CategoryChangedEvent message)
        {
            var seoSupport = message.ChangedEntries.Select(en => en.NewEntry).OfType<ISeoSupport>().ToArray();
            await _seoService.SaveSeoForObjectsAsync(seoSupport);
        }

        public Task Handle(CategoryChangingEvent message)
        {
            return Task.CompletedTask;
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Data.Handlers
{
    public class ProductChangedEventHandler : IEventHandler<ProductChangedEvent>, IEventHandler<ProductChangingEvent>
    {
        private readonly ISeoService _seoService;

        public ProductChangedEventHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public async Task Handle(ProductChangedEvent message)
        {
            var products = message.ChangedEntries.Select(en => en.NewEntry).ToArray();
            var productsWithVariations = products.Concat(products.Where(x => x.Variations != null).SelectMany(x => x.Variations)).OfType<ISeoSupport>().ToArray();
            await _seoService.SaveSeoForObjectsAsync(productsWithVariations);
        }

        public Task Handle(ProductChangingEvent message)
        {
            return Task.CompletedTask;
        }
    }
}

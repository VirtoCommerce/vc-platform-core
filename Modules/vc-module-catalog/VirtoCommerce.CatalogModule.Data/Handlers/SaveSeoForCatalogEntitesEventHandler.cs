using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Data.Handlers
{
    public class SaveSeoForCatalogEntitesEventHandler : IEventHandler<CategoryChangedEvent>, IEventHandler<ProductChangedEvent>
    {
        private readonly ISeoService _seoService;

        public SaveSeoForCatalogEntitesEventHandler(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public virtual async Task Handle(CategoryChangedEvent message)
        {
            var categories = message.ChangedEntries.Select(x => x.NewEntry).OfType<ISeoSupport>().ToArray();
            await SaveSeoForObjectsAsync(categories);
        }

        public virtual async Task Handle(ProductChangedEvent message)
        {
            var products = message.ChangedEntries.Select(en => en.NewEntry).ToArray();
            var productsWithVariations = products.Concat(products.Where(x => x.Variations != null).SelectMany(x => x.Variations)).OfType<ISeoSupport>().ToArray();
            await SaveSeoForObjectsAsync(productsWithVariations);
        }

        protected virtual async Task SaveSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            await _seoService.SaveSeoForObjectsAsync(seoSupportObjects);
        }
    }
}

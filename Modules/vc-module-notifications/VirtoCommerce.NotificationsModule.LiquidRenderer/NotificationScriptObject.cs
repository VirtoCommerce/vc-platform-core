using Scriban.Runtime;
using VirtoCommerce.NotificationsModule.LiquidRenderer.Filters;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Localizations;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer
{
    public class NotificationScriptObject : ScriptObject
    {
        public NotificationScriptObject(ITranslationService translationService, IBlobUrlResolver blobUrlResolver)
        {
            this.Import(typeof(TranslationFilter));
            this.Import(typeof(StandardFilters));
            this.Import(typeof(UrlFilters));

            TranslationService = translationService;
            BlobUrlResolver = blobUrlResolver;
        }
        public string Language { get; set; }
        public ITranslationService TranslationService { get; }
        public IBlobUrlResolver BlobUrlResolver { get; }
    }
}

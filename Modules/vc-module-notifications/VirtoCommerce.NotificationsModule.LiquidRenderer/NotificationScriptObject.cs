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

        public string Language
        {
            get
            {
                return GetSafeValue<string>(nameof(Language));
            }
            set
            {
                SetValue(nameof(Language), value, readOnly: true);
            }
        }

        public ITranslationService TranslationService
        {
            get
            {
                return GetSafeValue<ITranslationService>(nameof(TranslationService));
            }
            set
            {
                SetValue(nameof(TranslationService), value, readOnly: true);
            }
        }

        public IBlobUrlResolver BlobUrlResolver
        {
            get
            {
                return GetSafeValue<IBlobUrlResolver>(nameof(BlobUrlResolver));
            }
            set
            {
                SetValue(nameof(BlobUrlResolver), value, readOnly: true);
            }
        }
    }
}

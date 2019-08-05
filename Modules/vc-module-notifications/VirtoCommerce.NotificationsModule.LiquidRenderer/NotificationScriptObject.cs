using Scriban.Runtime;
using VirtoCommerce.NotificationsModule.LiquidRenderer.Filters;
using VirtoCommerce.Platform.Core.Localizations;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer
{
    public class NotificationScriptObject : ScriptObject
    {
        public NotificationScriptObject(ITranslationService translationService)
        {
            this.Import(typeof(TranslationFilter));
            this.Import(typeof(StandardFilters));

            TranslationService = translationService;
        }
        public string Language { get; set; }
        public ITranslationService TranslationService { get; }
    }
}

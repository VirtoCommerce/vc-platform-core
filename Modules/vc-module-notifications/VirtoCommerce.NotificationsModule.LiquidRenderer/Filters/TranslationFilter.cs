using Scriban;
using Scriban.Syntax;
using VirtoCommerce.Platform.Core.Localizations;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer.Filters
{
    public static class TranslationFilter
    {
        public static string Translate(TemplateContext context, string key, string language = null)
        {
            var result = key;

            var translationService = (ITranslationService)context.GetValue(new ScriptVariableGlobal(nameof(NotificationScriptObject.TranslationService)));

            if (string.IsNullOrEmpty(language))
            {
                language = context.GetValue(new ScriptVariableGlobal(nameof(NotificationScriptObject.Language)))?.ToString();
            }
            
            var translation = translationService.GetTranslationDataForLanguage(language);
            if (translation != null)
            {
                result = (translation.SelectToken(key) ?? key).ToString();                
            }
            return result;
        }
    }
}

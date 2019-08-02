using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Syntax;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer.Filters
{
    public static class TranslationFilter
    {
        public static string Translate(TemplateContext context, string path, string language = null)
        {
            var localizationResources = (JObject)context.GetValue(new ScriptVariableGlobal("localizationResources"));
            var languageObject = context.GetValue(new ScriptVariableGlobal("language"))?.ToString();
            var key = !string.IsNullOrEmpty(language) ? $"{language}.{path}" :
                !string.IsNullOrEmpty(languageObject) ? $"{languageObject}.{path}" :
                $"default.{path}";

            return (localizationResources?.SelectToken(key) ?? key).ToString();
        }
    }
}

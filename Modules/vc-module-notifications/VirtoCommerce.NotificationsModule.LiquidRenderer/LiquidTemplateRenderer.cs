using System.Globalization;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.LiquidRenderer.Filters;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Localizations;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer
{
    public class LiquidTemplateRenderer : INotificationTemplateRenderer
    {
        private readonly ILocalizationService _localizationService;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public LiquidTemplateRenderer(ILocalizationService localizationService, IBlobUrlResolver blobUrlResolver)
        {
            _localizationService = localizationService;
            _blobUrlResolver = blobUrlResolver;
        }

        public async Task<string> RenderAsync(string stringTemplate, object model, string language = null)
        {
            var context = new LiquidTemplateContext();
            var scriptObject = GenerateScriptObject();
            scriptObject.Import(model);
            if (!string.IsNullOrEmpty(language))
            {
                //TODO
                var culture = new CultureInfo(language);
                scriptObject.Add("language", culture.TwoLetterISOLanguageName);
            }
            context.PushGlobal(scriptObject);

            var template = Template.ParseLiquid(stringTemplate);
            var result = await template.RenderAsync(context);

            return result;
        }

        private ScriptObject GenerateScriptObject()
        {
            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(TranslationFilter));
            scriptObject.Import(typeof(StandardFilters));
            scriptObject.Import(typeof(UrlFilters));
            scriptObject.Add("localizationResources", _localizationService.GetResources());
            scriptObject.Add("IBlobUrlResolver", _blobUrlResolver);

            return scriptObject;
        }
    }
}

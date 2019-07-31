using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Localizations;

namespace VirtoCommerce.Platform.Data.Localizations
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder appBuilder)
        {
            var localizationService = appBuilder.ApplicationServices.GetRequiredService<ILocalizationService>();
            localizationService.FillLocalizationResources();

            return appBuilder;
        }
    }
}

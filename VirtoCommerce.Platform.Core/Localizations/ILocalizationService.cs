using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.Localizations
{
    public interface ILocalizationService
    {
        object GetLocalization(string lang = "en");
        string[] GetLocales();
        void FillLocalizationResources();
        object LocalizationResources { get; set; }
    }
}

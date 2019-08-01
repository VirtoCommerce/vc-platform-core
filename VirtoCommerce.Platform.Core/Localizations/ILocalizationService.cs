namespace VirtoCommerce.Platform.Core.Localizations
{
    public interface ILocalizationService
    {
        object GetByLanguage(string language = "en");
        string[] GetLocales();
        object GetResources();
    }
}

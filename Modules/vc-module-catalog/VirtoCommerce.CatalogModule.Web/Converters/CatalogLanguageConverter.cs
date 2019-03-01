using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class CatalogLanguageConverter
    {
        public static webModel.CatalogLanguage ToWebModel(this moduleModel.CatalogLanguage language)
        {
            return new webModel.CatalogLanguage
            {
                CatalogId = language.CatalogId,
                LanguageCode = language.LanguageCode,
                IsDefault = language.IsDefault
            };
        }


        public static moduleModel.CatalogLanguage ToCoreModel(this webModel.CatalogLanguage language)
        {
            return new moduleModel.CatalogLanguage
            {
                CatalogId = language.CatalogId,
                LanguageCode = language.LanguageCode,
                IsDefault = language.IsDefault
            };

        }
    }
}

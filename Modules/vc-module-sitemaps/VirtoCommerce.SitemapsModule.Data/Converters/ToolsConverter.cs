using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Converters
{
    public static class ToolsConverter
    {
        public static Tools.Models.Store ToToolsStore(this Store store, string baseUrl)
        {
            return new Tools.Models.Store
            {
                Catalog = store.Catalog,
                DefaultLanguage = store.DefaultLanguage,
                Id = store.Id,
                Languages = store.Languages.ToList(),
                SecureUrl = !string.IsNullOrEmpty(store.SecureUrl) ? store.SecureUrl : baseUrl,
                SeoLinksType = GetSeoLinksType(store),
                Url = !string.IsNullOrEmpty(store.Url) ? store.Url : baseUrl
            };
        }

        private static Tools.Models.SeoLinksType GetSeoLinksType(Store store)
        {
            var seoLinksType = Tools.Models.SeoLinksType.Collapsed;

            if (store.Settings != null)
            {
                var seoLinksTypeSetting = store.Settings.FirstOrDefault(s => s.Name == "Stores.SeoLinksType");
                if (seoLinksTypeSetting != null)
                {
                    seoLinksType = EnumUtility.SafeParse(seoLinksTypeSetting.Value?.ToString(), Tools.Models.SeoLinksType.Collapsed);
                }
            }

            return seoLinksType;
        }
    }
}

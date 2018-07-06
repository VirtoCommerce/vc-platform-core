using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Converters
{
    //ToDo use virtocommerce.tools here
    public enum SeoLinksType
    {
        None = 0,
        Short = 1,
        Long = 2,
        Collapsed = 3
    }

    public class Store
    {

        public string Id { get; set; }
        public string Url { get; set; }
        public string SecureUrl { get; set; }
        public string Catalog { get; set; }
        public string DefaultLanguage { get; set; }
        public SeoLinksType SeoLinksType { get; set; }
        public IList<string> Languages { get; set; }
    }

    public static class ToolsConverter
    {

        public static Store ToToolsStore(this VirtoCommerce.StoreModule.Core.Model.Store store, string baseUrl)
        {
            return new Store
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

        private static SeoLinksType GetSeoLinksType(VirtoCommerce.StoreModule.Core.Model.Store store)
        {
            var seoLinksType = SeoLinksType.Collapsed;

            if (store.Settings != null)
            {
                var seoLinksTypeSetting = store.Settings.FirstOrDefault(s => s.Name == "Stores.SeoLinksType");
                if (seoLinksTypeSetting != null)
                {
                    seoLinksType = EnumUtility.SafeParse(seoLinksTypeSetting.Value, SeoLinksType.Collapsed);
                }
            }

            return seoLinksType;
        }
    }
}

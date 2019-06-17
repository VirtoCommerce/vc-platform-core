using System.Linq;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Converters;
using VirtoCommerce.SitemapsModule.Data.Extensions;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.Tools;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapUrlBuilder : ISitemapUrlBuilder
    {
        public SitemapUrlBuilder(IUrlBuilder urlBuilder)
        {
            UrlBuilder = urlBuilder;
        }

        protected IUrlBuilder UrlBuilder { get; private set; }

        #region ISitemapUrlBuilder members
        public virtual string BuildStoreUrl(Store store, string language, string urlTemplate, string baseUrl, IEntity entity = null)
        {
            var toolsStore = store.ToToolsStore(baseUrl);

            var seoSupport = entity as ISeoSupport;

            //remove unused {language} template
            urlTemplate = urlTemplate.Replace(UrlTemplatePatterns.Language, string.Empty);

            var slug = string.Empty;
            if (seoSupport != null)
            {
                var hasOutlines = entity as IHasOutlines;
                var seoInfos = seoSupport.SeoInfos?.Select(x => x.JsonConvert<Tools.Models.SeoInfo>());
                seoInfos = seoInfos?.GetBestMatchingSeoInfos(toolsStore.Id, toolsStore.DefaultLanguage, language, null);
                if (!seoInfos.IsNullOrEmpty())
                {
                    slug = seoInfos.Select(x => x.SemanticUrl).FirstOrDefault();
                }
                if (hasOutlines != null && !hasOutlines.Outlines.IsNullOrEmpty())
                {
                    var outlines = hasOutlines.Outlines.Select(x => x.JsonConvert<Tools.Models.Outline>());
                    slug = outlines.GetSeoPath(toolsStore, language, slug);
                }
            }
            var toolsContext = new Tools.Models.UrlBuilderContext
            {
                AllStores = new[] { toolsStore },
                CurrentLanguage = language,
                CurrentStore = toolsStore
            };
            //Replace {slug} template in passed url template
            urlTemplate = urlTemplate.Replace(UrlTemplatePatterns.Slug, slug);
            var result = UrlBuilder.BuildStoreUrl(toolsContext, urlTemplate);
            return result;
        }
        #endregion
    }
}

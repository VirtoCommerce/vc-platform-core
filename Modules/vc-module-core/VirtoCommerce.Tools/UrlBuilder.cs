using System;
using System.Linq;
using System.Text;
using VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Tools
{
    public class UrlBuilder : IUrlBuilder
    {
        public virtual string BuildStoreUrl(UrlBuilderContext context, string virtualPath)
        {
            return BuildStoreUrl(context, virtualPath, context?.CurrentStore, context?.CurrentLanguage);
        }

        public virtual string BuildStoreUrl(UrlBuilderContext context, string virtualPath, Store store, string language)
        {
            var result = virtualPath;

            // Don't process absolute URL
            Uri absoluteUri;
            if (virtualPath != null && !Uri.TryCreate(virtualPath, UriKind.Absolute, out absoluteUri))
            {
                var builder = new StringBuilder("~");

                if (store != null)
                {
                    // If store has public or secure URL, use them
                    if (!string.IsNullOrEmpty(store.Url) || !string.IsNullOrEmpty(store.SecureUrl))
                    {
                        string baseAddress = null;

                        // If current request is secure, use secure URL
                        if (!string.IsNullOrEmpty(context?.CurrentUrl) &&
                            !string.IsNullOrEmpty(store.SecureUrl) &&
                            context.CurrentUrl.StartsWith(store.SecureUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            baseAddress = store.SecureUrl;
                        }

                        if (baseAddress == null)
                        {
                            baseAddress = !string.IsNullOrEmpty(store.Url) ? store.Url : store.SecureUrl;
                        }

                        builder.Clear();
                        builder.Append(baseAddress.TrimEnd('/'));
                    }
                    else
                    {
                        // Do not add storeId to URL if there is only one store
                        if (context?.AllStores?.Count > 1)
                        {
                            // If specified store does not exist, use current store
                            store = context.AllStores.Any(s => s.Id.EqualsInvariant(store.Id)) ? store : context.CurrentStore;

                            if (!virtualPath.Contains("/" + store.Id + "/"))
                            {
                                builder.Append("/");
                                builder.Append(store.Id);
                            }
                        }
                    }

                    // Do not add language to URL if store has only one language
                    if (language != null && store.Languages?.Count > 1)
                    {
                        language = store.Languages.Contains(language) ? language : store.DefaultLanguage;
                        if (!virtualPath.Contains("/" + language + "/"))
                        {
                            builder.Append("/");
                            builder.Append(language);
                        }
                    }
                }

                builder.Append("/");
                builder.Append(virtualPath.TrimStart('~', '/'));
                result = builder.ToString();
            }

            return result;
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Web;
using Scriban;
using Scriban.Syntax;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/url-filters
    /// </summary>
    public static partial class UrlFilters
    {
        /// <summary>
        /// Returns the URL of a file in the "assets" folder of a theme.
        /// {{ 'shop.css' | asset_url }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AssetUrl(TemplateContext context, string input)
        {
            string retVal = null;
            if (input != null)
            {
                var blobUrlResolver = (IBlobUrlResolver)context.GetValue(new ScriptVariableGlobal(nameof(NotificationScriptObject.BlobUrlResolver)));
                retVal = blobUrlResolver.GetAbsoluteUrl(input);
            }
            return retVal;
        }
    }
}

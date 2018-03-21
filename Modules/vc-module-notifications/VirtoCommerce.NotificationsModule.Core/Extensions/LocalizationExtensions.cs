using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Extensions
{
    public static class LocalizationExtensions
    {
        public static T FindWithLanguage<T>(this IEnumerable<T> items, string language) where T : IHasLanguageCode
        {
            if (string.IsNullOrEmpty(language)) language = null;

            var result = items.FirstOrDefault(i => i.LanguageCode == language);
            
            return result;
        }
    }
}

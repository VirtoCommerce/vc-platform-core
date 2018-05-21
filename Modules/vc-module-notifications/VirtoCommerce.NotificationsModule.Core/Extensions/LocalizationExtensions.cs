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
            return items.Where(x => x.LanguageCode == null || x.LanguageCode.EqualsInvariant(language))
                .OrderByDescending(x => x.LanguageCode).FirstOrDefault(); ;
        }
    }
}

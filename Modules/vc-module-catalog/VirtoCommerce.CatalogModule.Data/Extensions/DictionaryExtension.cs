using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CatalogModule.Data.Extensions
{
    public static class DictionaryExtension
    {
        public static TValue GetValueOrThrow<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, string exceptionMessage)
        {
            TValue value;
            if(!dictionary.TryGetValue(key, out value))
            {
                throw new KeyNotFoundException(exceptionMessage);
            }
            return value;
        }
    }
}

using Newtonsoft.Json.Linq;

namespace VirtoCommerce.SitemapsModule.Data.Extensions
{
    public static class ObjectExtensions
    {
        public static T JsonConvert<T>(this object source)
        {
            var jObject = JObject.FromObject(source);
            var result = jObject.ToObject<T>();
            return result;
        }
    }
}

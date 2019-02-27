using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public static class IndexDocumentHelper
    {
        public const string ObjectFieldName = "__object";

        public static void AddObjectFieldValue<T>(this IndexDocument document, T value)
        {
            document.Add(new IndexDocumentField(ObjectFieldName, SerializeObject(value)) { IsRetrievable = true });
        }

        public static T GetObjectFieldValue<T>(this SearchDocument document)
            where T : class
        {
            T result = null;

            if (document.ContainsKey(ObjectFieldName))
            {
                var obj = document[ObjectFieldName];

                result = obj as T;
                if (result == null)
                {
                    var jobj = obj as JObject;
                    if (jobj != null)
                    {
                        result = jobj.ToObject<T>();
                    }
                    else
                    {
                        var productString = obj as string;
                        if (!string.IsNullOrEmpty(productString))
                        {
                            result = DeserializeObject<T>(productString);
                        }
                    }
                }
            }

            return result;
        }


        public static JsonSerializer ObjectSerializer { get; } = new JsonSerializer
        {
            DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
        };

        public static string SerializeObject(object obj)
        {
            using (var memStream = new MemoryStream())
            {
                obj.SerializeJson(memStream, ObjectSerializer);
                memStream.Seek(0, SeekOrigin.Begin);

                var result = memStream.ReadToString();
                return result;
            }
        }

        public static T DeserializeObject<T>(string str)
        {
            using (var stringReader = new StringReader(str))
            using (var jsonTextReader = new JsonTextReader(stringReader))
            {
                var result = ObjectSerializer.Deserialize<T>(jsonTextReader);
                return result;
            }
        }
    }
}

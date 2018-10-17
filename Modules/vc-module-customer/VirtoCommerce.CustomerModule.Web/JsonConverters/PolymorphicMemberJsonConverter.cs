using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Web.JsonConverters
{
    /// <summary>
    /// Used to deserialize from JSON derived Member types
    /// </summary>
    public class PolymorphicMemberJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(Member).IsAssignableFrom(objectType) || objectType == typeof(MembersSearchCriteria);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            if (typeof(Member).IsAssignableFrom(objectType))
            {
                var memberType = objectType.Name;
                var pt = obj["memberType"] ?? obj["MemberType"];
                if (pt != null)
                {
                    memberType = pt.Value<string>();
                }
                retVal = AbstractTypeFactory<Member>.TryCreateInstance(memberType);
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown memberType: " + memberType);
                }

            }
            else if (objectType == typeof(MembersSearchCriteria))
            {
                retVal = AbstractTypeFactory<MembersSearchCriteria>.TryCreateInstance();
            }
            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

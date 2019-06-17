using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.NotificationsModule.Web.Model
{
    public class NotificationRequest
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectTypeId")]
        public string ObjectTypeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "notificationParameters")]
        public IList<NotificationParameter> NotificationParameters { get; set; }
    }
}

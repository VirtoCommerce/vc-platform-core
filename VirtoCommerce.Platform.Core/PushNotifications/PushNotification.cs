using System;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.PushNotifications
{
    public abstract class PushNotification : IEntity
    {
        public PushNotification(string creator)
        {
            Created = DateTime.UtcNow;
            IsNew = true;
            Id = Guid.NewGuid().ToString();
            Creator = creator;
            NotifyType = GetType().Name;
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("creator")]
        public string Creator { get; set; }
        [JsonProperty("created")]
        public DateTime Created { get; set; }
        [JsonProperty("isNew")]
        public bool IsNew { get; set; }
        [JsonProperty("notifyType")]
        public string NotifyType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("repeatCount")]
        public int RepeatCount { get; set; }

        public bool ItHasSameContent(PushNotification other)
        {
            return other.Title == Title && other.NotifyType == NotifyType && other.Description == Description;
        }

    }
}

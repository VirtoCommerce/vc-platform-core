using System;

namespace VirtoCommerce.Platform.Redis
{
    [Serializable]
    public class CachingMessage
    {
        public string Id { get; set; }

        public string[] CacheKeys { get; set; }

        public bool IsPrefix { get; set; }
    }
}

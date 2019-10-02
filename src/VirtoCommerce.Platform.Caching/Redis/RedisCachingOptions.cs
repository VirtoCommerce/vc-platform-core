namespace VirtoCommerce.Platform.Redis
{
    public class RedisCachingOptions
    {
        public string ConnectionString { get; set; }
        public string ConfigurationKey { get; set; }
        public string ChannelName { get; set; }
    }
}

namespace VirtoCommerce.Platform.Core.Caching
{
    public class RedisCachingOptions
    {
        public bool IsEnabled { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigurationKey { get; set; }
    }
}

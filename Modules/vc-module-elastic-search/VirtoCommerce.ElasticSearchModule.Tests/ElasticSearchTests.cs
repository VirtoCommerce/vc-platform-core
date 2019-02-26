using System;
using VirtoCommerce.ElasticSearchModule.Data;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Tests;
using Xunit;

namespace VirtoCommerce.ElasticSearchModule.Tests
{
    [Trait("Category", "CI")]
    public class ElasticSearchTests : SearchProviderTests
    {
        protected override ISearchProvider GetSearchProvider()
        {
            var host = Environment.GetEnvironmentVariable("TestElasticsearchHost") ?? "localhost:9200";
            var provider = new ElasticSearchProvider(new SearchConnection($"server=http://{host};scope=test", "Elasticsearch"), GetSettingsManager());
            return provider;
        }
    }
}

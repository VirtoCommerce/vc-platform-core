using System;
using Microsoft.Extensions.Options;
using VirtoCommerce.ElasticSearchModule.Data;
using VirtoCommerce.LuceneSearchModule.Data;
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

            var elasticOptions = Options.Create(new ElasticSearchOptions {  Server = host });
            var searchOptions = Options.Create(new SearchOptions { Scope = "test-core", Provider = "ElasticSearch" });

            var provider = new ElasticSearchProvider(elasticOptions, searchOptions, GetSettingsManager());
            return provider;
        }
    }
}

using System;
using Microsoft.Extensions.Options;
using VirtoCommerce.AzureSearchModule.Data;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Tests;
using Xunit;

namespace VirtoCommerce.AzureSearchModule.Tests
{
    [Trait("Category", "CI")]
    public class AzureSearchTests : SearchProviderTests
    {
        protected override ISearchProvider GetSearchProvider()
        {
            var searchServiceName = Environment.GetEnvironmentVariable("TestAzureSearchServiceName") ?? "Test SearchServiceName";
            var key = Environment.GetEnvironmentVariable("TestAzureSearchKey") ?? "Test key";

            var azureSearchOptions = Options.Create(new AzureSearchOptions { SearchServiceName = searchServiceName, Key = key });
            var options = Options.Create(new SearchOptions { Scope = "test-core", Provider = "AzureSearch" });

            var provider = new AzureSearchProvider(azureSearchOptions, options, GetSettingsManager());
            return provider;
        }
    }
}

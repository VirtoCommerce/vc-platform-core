using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.LuceneSearchModule.Data;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Tests;
using Xunit;

namespace VirtoCommerce.LuceneSearchModule.Tests
{
    [Trait("Category", "CI")]
    public class LuceneSearchTests : SearchProviderTests
    {
        private readonly string _dataDirectoryPath = Path.Combine(Path.GetTempPath(), "lucene");

        protected override ISearchProvider GetSearchProvider()
        {
            var someOptions = Options.Create(new LuceneSearchSettings() { Path = _dataDirectoryPath, Scope = "test" });
            return new LuceneSearchProvider(someOptions);
        }
    }
}

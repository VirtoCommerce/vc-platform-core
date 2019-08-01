using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Localizations;
using Xunit;

namespace VirtoCommerce.Platform.Tests.IntegrationTests
{
    public class LocalizationServiceIntegrationTests
    {
        private readonly Mock<ILocalModuleCatalog> _localModuleCatalogMock;
        private readonly Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;

        private readonly LocalizationService _localizationService;
        public LocalizationServiceIntegrationTests()
        {
            _localModuleCatalogMock = new Mock<ILocalModuleCatalog>();
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _cacheEntryMock = new Mock<ICacheEntry>();
            _cacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            var dir = @"..\..\..\";
            _hostingEnvironmentMock.Setup(x => x.WebRootPath).Returns(dir);

            _localizationService = new LocalizationService(_localModuleCatalogMock.Object, _hostingEnvironmentMock.Object, _platformMemoryCacheMock.Object);
        }

        [Fact]
        public void GetResources_SelectToken_Success()
        {
            //Arrange

            var cacheKey = CacheKey.With(_localizationService.GetType(), "GetAllLocalizationFiles", "*.json", "Localizations");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);
            var enCacheKey = CacheKey.With(_localizationService.GetType(), "GetAllLocalizationFiles", "en.*.json", "Localizations");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(enCacheKey)).Returns(_cacheEntryMock.Object);
            var enLangCacheKey = CacheKey.With(_localizationService.GetType(), "GetByLanguage", "en");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(enLangCacheKey)).Returns(_cacheEntryMock.Object);

            //Act
            var result = (JObject)_localizationService.GetResources();
            var token = result.SelectToken("en.platform.commands.test");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.HasValues);
            Assert.Equal("Test success", token.ToString());
        }

        [Fact]
        public void GetLocales_ReturnEnglish()
        {
            //Arrange
            var cacheKey = CacheKey.With(_localizationService.GetType(), "GetAllLocalizationFiles", "*.json", "Localizations");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);

            //Act
            var result = _localizationService.GetLocales();

            //Assert
            Assert.NotNull(result);
            Assert.Contains("en", result);
        }

        [Fact]
        public void GetByLanguage_English()
        {
            //Arrange
            var cacheKey = CacheKey.With(_localizationService.GetType(), "GetAllLocalizationFiles", "en.*.json", "Localizations");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);
            var enLangCacheKey = CacheKey.With(_localizationService.GetType(), "GetByLanguage", "en");
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(enLangCacheKey)).Returns(_cacheEntryMock.Object);

            //Act
            var result = (JObject)_localizationService.GetByLanguage();
            var token = result.SelectToken("platform.commands.test");

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test success", token.ToString());
        }
    }
}

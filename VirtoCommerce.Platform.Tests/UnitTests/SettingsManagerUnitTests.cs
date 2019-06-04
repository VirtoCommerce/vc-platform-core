using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Settings;
using Xunit;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class SettingsManagerUnitTests
    {
        private readonly SettingsManager _settingsManager;
        private readonly Mock<IPlatformRepository> _repositoryMock;
        private Func<IPlatformRepository> _repositoryFactory;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<ICacheEntry> _сacheEntryMock;

        public SettingsManagerUnitTests()
        {
            _repositoryMock = new Mock<IPlatformRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _repositoryFactory = () => _repositoryMock.Object;
            _memoryCacheMock = new Mock<IMemoryCache>();
            _сacheEntryMock = new Mock<ICacheEntry>();
            _сacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            _settingsManager = new SettingsManager(_repositoryFactory, _memoryCacheMock.Object);
        }

        [Fact]
        public async Task GetValueAsync_ValueGreaterZero_ReturnValue()
        {
            //Arrange
            var integerValue = 25;
            var settingsName = "Platform.Setting.Name";
            var settings = new List<SettingDescriptor>()
            {
                new SettingDescriptor
                {
                    Name = settingsName,
                    GroupName = "Platform|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 50
                }
            };
            _settingsManager.RegisterSettings(settings);
            _repositoryMock.Setup(r => r.GetObjectSettingsAsync(null, null)).ReturnsAsync(new []
            {
                new SettingEntity { Id = Guid.NewGuid().ToString()
                    , Name = settingsName
                    , SettingValues = new ObservableCollection<SettingValueEntity>()
                        { new SettingValueEntity() { ValueType = SettingValueType.Integer.ToString(), IntegerValue = integerValue } }},
            });
            var cacheKey = CacheKey.With(_settingsManager.GetType(), "GetSettingByNamesAsync", settingsName, null, null);

            _memoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_сacheEntryMock.Object);

            //Act
            var result = await _settingsManager.GetValueAsync(settingsName, 50);

            //Assert
            Assert.Equal(integerValue, result);
        }

        [Theory]
        [InlineData(SettingValueType.Integer, 50, 25, 25)]
        [InlineData(SettingValueType.Integer, 50, 50, 0)]
        [InlineData(SettingValueType.Decimal, 50, 25, 25)]
        [InlineData(SettingValueType.Decimal, 50, 50, 0)]
        [InlineData(SettingValueType.ShortText, "", "test", "test")]
        public async Task GetValueAsync_ReturnValue(SettingValueType settingValueType, object defaultValue, object expectedValue, object actualValue)
        {
            //Arrange
            var settingsName = "Platform.Setting.Name";
            var settings = new List<SettingDescriptor>()
            {
                new SettingDescriptor
                {
                    Name = settingsName,
                    GroupName = "Platform|General",
                    ValueType = settingValueType,
                    DefaultValue = defaultValue
                }
            };

            var settingValue = new SettingValueEntity();
            settingValue.SetValue(settingValueType, actualValue);
            
            _settingsManager.RegisterSettings(settings);
            _repositoryMock.Setup(r => r.GetObjectSettingsAsync(null, null)).ReturnsAsync(new[]
            {
                new SettingEntity { Id = Guid.NewGuid().ToString()
                    , Name = settingsName
                    , SettingValues = new ObservableCollection<SettingValueEntity> { settingValue }},
            });

            var cacheKey = CacheKey.With(_settingsManager.GetType(), "GetSettingByNamesAsync", settingsName, null, null);
            _memoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_сacheEntryMock.Object);

            //Act
            var result = await _settingsManager.GetValueAsync(settingsName, defaultValue);

            //Assert
            Assert.Equal(expectedValue, result);
        }
    }
}

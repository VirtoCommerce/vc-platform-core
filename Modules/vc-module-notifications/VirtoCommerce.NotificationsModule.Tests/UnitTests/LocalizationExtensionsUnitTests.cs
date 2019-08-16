using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class LocalizationExtensionsUnitTests
    {
        private readonly List<IHasLanguageCode> _templates;
        public LocalizationExtensionsUnitTests()
        {
            _templates = new List<IHasLanguageCode>()
            {
                new SomeTemplate { LanguageCode = null },
                new SomeTemplate { LanguageCode = "en-US" },
                new SomeTemplate { LanguageCode = "de-DE" },
            };
        }

        [Fact]
        public void FindWithLanguage_WhenIsNull_ReturnDefaultLanguage()
        {
            //Arrange
            string languageCode = null;

            //Act
            var result = _templates.FindWithLanguage(languageCode);

            //Assert
            Assert.NotNull(result);
            Assert.Null(result.LanguageCode);
        }

        [Fact]
        public void FindWithLanguage_WhenIsEmpty_ReturnDefaultLanguage()
        {
            //Arrange
            string languageCode = String.Empty;

            //Act
            var result = _templates.FindWithLanguage(languageCode);

            //Assert
            Assert.NotNull(result);
            Assert.Null(result.LanguageCode);
        }

        [Fact]
        public void FindWithLanguage_WhenIsEnUS_ReturnLanguageEnUS()
        {
            //Arrange
            string languageCode = "en-US";

            //Act
            var result = _templates.FindWithLanguage(languageCode);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(languageCode, result.LanguageCode);
        }

        [Fact]
        public void FindWithLanguage_WhenWrongCode_ReturnDefaultLanguage()
        {
            //Arrange
            string languageCode = "fr-FR";

            //Act
            var result = _templates.FindWithLanguage(languageCode);

            //Assert
            Assert.NotNull(result);
            Assert.Null(result.LanguageCode);
        }
    }

    public class SomeTemplate : IHasLanguageCode
    {
        public string LanguageCode { get; set; }
    }
}

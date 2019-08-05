using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Localizations;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class LiquidTemplateRendererUnitTests
    {
        private readonly Mock<ILocalizationService> _localizationServiceMock;
        private readonly Mock<IBlobUrlResolver> _blobUrlResolverMock;
        private readonly LiquidTemplateRenderer _liquidTemplateRenderer;
        public LiquidTemplateRendererUnitTests()
        {
            _localizationServiceMock = new Mock<ILocalizationService>();
            _blobUrlResolverMock = new Mock<IBlobUrlResolver>();
            _liquidTemplateRenderer = new LiquidTemplateRenderer(_localizationServiceMock.Object, _blobUrlResolverMock.Object);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("en")]
        public void Render_TranslateEnglish(string language)
        {
            //Arrange
            _blobUrlResolverMock.Setup(x => x.GetAbsoluteUrl(It.IsAny<string>())).Returns("http://localhost:10645/assets/");
            var jObject = JObject.FromObject(new { en = new { order = new { subject1 = "subj" } } });
            _localizationServiceMock.Setup(x => x.GetResources()).Returns(jObject);
            string input = "{{ 'order.subject1' | translate language }} test {{ 'notifications/1.svg' | asset_url }}";

            //Act
            var result = _liquidTemplateRenderer.RenderAsync(input, null, language).GetAwaiter().GetResult();

            //Assert
            Assert.Equal("subj test", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Render_TranslateAsDefault(string language)
        {
            //Arrange
            var jObject = JObject.FromObject(new { @default = new { order = new { subject1 = "subj" } } });
            _localizationServiceMock.Setup(x => x.GetResources()).Returns(jObject);
            string input = "{{ 'order.subject1' | translate language }} test";

            //Act
            var result = _liquidTemplateRenderer.RenderAsync(input, null, language).GetAwaiter().GetResult();

            //Assert
            Assert.Equal("subj test", result);
        }

        [Fact]
        public void Render_GetAssetUrl()
        {
            //Arrange
            _blobUrlResolverMock.Setup(x => x.GetAbsoluteUrl(It.IsAny<string>())).Returns("http://localhost:10645/assets/1.svg");
            var jObject = JObject.FromObject(new { });
            _localizationServiceMock.Setup(x => x.GetResources()).Returns(jObject);
            string input = "test {{ '1.svg' | asset_url }}";

            //Act
            var result = _liquidTemplateRenderer.RenderAsync(input, null).GetAwaiter().GetResult();

            //Assert
            Assert.Equal("test http://localhost:10645/assets/1.svg", result);
        }

    }

    
}

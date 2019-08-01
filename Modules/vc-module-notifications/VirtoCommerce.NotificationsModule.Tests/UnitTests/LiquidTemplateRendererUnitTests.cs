using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.Platform.Core.Localizations;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class LiquidTemplateRendererUnitTests
    {
        private readonly Mock<ILocalizationService> _localizationServiceMock;
        private readonly LiquidTemplateRenderer _liquidTemplateRenderer;
        public LiquidTemplateRendererUnitTests()
        {
            _localizationServiceMock = new Mock<ILocalizationService>();
            _liquidTemplateRenderer = new LiquidTemplateRenderer(_localizationServiceMock.Object);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("en")]
        public void Render_TranslateEnglish(string language)
        {
            //Arrange
            var jObject = JObject.FromObject(new { en = new { order = new { subject1 = "subj" } } });
            _localizationServiceMock.Setup(x => x.GetResources()).Returns(jObject);
            string input = "{{ 'order.subject1' | translate language }} test";

            //Act
            var result = _liquidTemplateRenderer.RenderAsync(input, null, language).GetAwaiter().GetResult();

            //Assert
            Assert.Equal("subj test", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Render_TranslateAsEmpty(string language)
        {
            //Arrange
            var jObject = JObject.FromObject(new { order = new { subject1 = "subj" } } );
            _localizationServiceMock.Setup(x => x.GetResources()).Returns(jObject);
            string input = "{{ 'order.subject1' | translate language }} test";

            //Act
            var result = _liquidTemplateRenderer.RenderAsync(input, null, language).GetAwaiter().GetResult();

            //Assert
            Assert.Equal("subj test", result);
        }



    }

    
}

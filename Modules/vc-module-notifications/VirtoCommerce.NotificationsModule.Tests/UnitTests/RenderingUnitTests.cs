using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using VirtoCommerce.NotificationsModule.LiquidRenderer.Filters;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class RenderingUnitTests
    {
        [Fact]
        public void Render_TranslateFilter_TranslateSubject()
        {
            //Arrange
            string input = "{{ 'order.subject1' | translate language }} test";
            var template = Template.Parse(input);
            var context = new TemplateContext();
            var scriptObject1 = new ScriptObject();
            scriptObject1.Add("language", "en");
            scriptObject1.Import(typeof(TranslationFilter));
            var jObject = JObject.FromObject(new { en = new { order = new { subject1 = "subj" }}});
            scriptObject1.Add("localizationResources", jObject);
            context.PushGlobal(scriptObject1);

            //Act
            var result = template.Render(context);

            //Assert
            Assert.Equal("subj test", result);
        }
        
    }

    
}

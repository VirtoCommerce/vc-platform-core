using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VirtoCommerce.Platform.Modules.Bundling;

namespace VirtoCommerce.Platform.Web.TagHelpers
{
    public class ModulesStylesResolveTagHelper : TagHelper
    {
        private readonly IStyleCollector _styleCollector;
        private ModuleFile[] _moduleStyles;

        [HtmlAttributeName("asp-append-version")]
        public bool AppendVersion { get; set; }

        public ModulesStylesResolveTagHelper(IStyleCollector styleCollector)
        {
            _styleCollector = styleCollector;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            _moduleStyles = _styleCollector.Collect(AppendVersion);

            BuildOutput(output);
        }

        private void BuildOutput(TagHelperOutput output)
        {
            foreach (var moduleStyle in _moduleStyles.Where(s => s.IsVendor).AsEnumerable())
            {
                AddTag(output, moduleStyle.WebPath, moduleStyle.Version);
            }

            foreach (var moduleStyle in _moduleStyles.Where(s => !s.IsVendor).AsEnumerable())
            {
                AddTag(output, moduleStyle.WebPath, moduleStyle.Version);
            }
        }

        private void AddTag(TagHelperOutput output, string path, string version)
        {
            var tagBuilder = new TagBuilder("link");

            tagBuilder.Attributes.Add("href", version != null ? $"{path}?v={version}" : path);
            tagBuilder.Attributes.Add("rel", "stylesheet");
            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;

            output.Content.AppendHtml(tagBuilder);
        }
    }
}

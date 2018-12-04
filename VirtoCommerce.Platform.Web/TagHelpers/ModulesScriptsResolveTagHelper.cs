using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VirtoCommerce.Platform.Core.ModuleFileCollector;

namespace VirtoCommerce.Platform.Web.TagHelpers
{
    public class ModulesScriptsResolveTagHelper : TagHelper
    {
        private readonly IScriptCollector _scriptCollector;
        private ModuleFile[] _moduleScripts;

        public ModulesScriptsResolveTagHelper(IScriptCollector scriptCollector)
        {
            _scriptCollector = scriptCollector;
        }

        [HtmlAttributeName("asp-append-version")]
        public bool AppendVersion { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            _moduleScripts = _scriptCollector.Collect(AppendVersion);
            
            BuildOutput(output);
        }

        private void BuildOutput(TagHelperOutput output)
        {
            foreach (var moduleScript in _moduleScripts.Where(s => s.IsVendor).AsEnumerable())
            {
                AddTag(output, moduleScript.WebPath, moduleScript.Version);
            }

            foreach (var moduleScript in _moduleScripts.Where(s => !s.IsVendor).AsEnumerable())
            {
                AddTag(output, moduleScript.WebPath, moduleScript.Version);
            }
        }

        private void AddTag(TagHelperOutput output, string path, string version)
        {
            var tagBuilder = new TagBuilder("script");

            tagBuilder.Attributes.Add("src", version != null ? $"{path}?v={version}" : path);
            tagBuilder.Attributes.Add("type", "text/javascript");

            output.Content.AppendHtml(tagBuilder);
        }
    }
}

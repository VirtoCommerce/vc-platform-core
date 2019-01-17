using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Modules.Bundling;

namespace VirtoCommerce.Platform.Web.TagHelpers
{
    public class BundleProviderTagHelper : TagHelper
    {
        private readonly IBundleProvider _bundleProvider;
        private readonly ILocalModuleCatalog _localModuleCatalog;

        private ManifestModuleInfo[] _includedModules;
        private TagHelperOutput _output;

        private ManifestModuleInfo[] IncludedModules =>
            _includedModules ?? (_includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray());

        [HtmlAttributeName("asp-append-version")]
        public bool AppendVersion { get; set; }

        [HtmlAttributeName("bundle-type")]
        public string BundleType { get; set; }

        public BundleProviderTagHelper(ILocalModuleCatalog localModuleCatalog, IBundleProvider bundleProvider)
        {
            _localModuleCatalog = localModuleCatalog;
            _bundleProvider = bundleProvider;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            if (string.IsNullOrEmpty(BundleType))
            {
                throw new ArgumentNullException(nameof(BundleType));
            }

            _output = output;

            if (BundleType == "styles")
            {
                var moduleStyles = _bundleProvider.CollectStyles(IncludedModules, AppendVersion);

                BuildOutputStyles(moduleStyles);
            }

            if (BundleType == "scripts")
            {
                var moduleScripts = _bundleProvider.CollectScripts(IncludedModules, AppendVersion);

                BuildOutputScripts(moduleScripts);
            }
        }

        private void BuildOutputStyles(ModuleFile[] moduleFiles)
        {
            foreach (var moduleStyle in moduleFiles)
            {
                var tagBuilder = new TagBuilder("link");

                tagBuilder.Attributes.Add("href", moduleStyle.Version != null ? $"{moduleStyle.WebPath}?v={moduleStyle.Version}" : moduleStyle.WebPath);
                tagBuilder.Attributes.Add("rel", "stylesheet");
                tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;

                _output.Content.AppendHtml(tagBuilder);
            }
        }

        private void BuildOutputScripts(ModuleFile[] moduleFiles)
        {
            foreach (var moduleScript in moduleFiles)
            {
                var tagBuilder = new TagBuilder("script");

                tagBuilder.Attributes.Add("src",
                    moduleScript.Version != null
                        ? $"{moduleScript.WebPath}?v={moduleScript.Version}"
                        : moduleScript.WebPath);
                tagBuilder.Attributes.Add("type", "text/javascript");

                _output.Content.AppendHtml(tagBuilder);
            }
        }
    }
}

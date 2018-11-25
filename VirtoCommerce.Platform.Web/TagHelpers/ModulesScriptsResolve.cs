using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VirtoCommerce.Platform.Core.FileVersionProvider;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Normalizer;

namespace VirtoCommerce.Platform.Web.TagHelpers
{
    public class ModulesScriptsResolveTagHelper : TagHelper
    {
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly ILocalModuleCatalog _localModuleCatalog;
        private readonly IModuleScriptPathNormalizerFactory _moduleScriptPathNormalizerFactory;

        public ModulesScriptsResolveTagHelper(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog, IModuleScriptPathNormalizerFactory moduleScriptPathNormalizerFactory)
        {
            _fileVersionProvider = fileVersionProvider;
            _localModuleCatalog = localModuleCatalog;
            _moduleScriptPathNormalizerFactory = moduleScriptPathNormalizerFactory;
        }
        private readonly ICollection<Script> _moduleScripts = new List<Script>();

        [HtmlAttributeName("asp-append-version")]
        public bool AppendVersion { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            LoadModulesTargetPaths();

            AddFileVersion();
            
            BuildOutput(output);
        }

        private void LoadModulesTargetPaths()
        {
            var includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().ToList();

            foreach (var includedModule in includedModules)
            {
                var scriptsMetadata = includedModule.Scripts.SingleOrDefault();

                if (null == scriptsMetadata)
                {
                    continue;
                }

                var scriptsFolderName = scriptsMetadata.VirtualPath.Split("/").Last();
                
                var targetPath = Path.Join(includedModule.FullPhysicalPath, scriptsFolderName, "dist");

                if (Directory.Exists(targetPath))
                {
                    var moduleScript = Path.Join(targetPath, "app.js");
                    var moduleVendor = Path.Join(targetPath, "vendor.js");

                    var normalizer = _moduleScriptPathNormalizerFactory.Create(scriptsMetadata.VirtualPath, includedModule.ModuleName, includedModule.Assembly.GetName().Name);

                    if (File.Exists(moduleScript))
                    {
                        _moduleScripts.Add(new Script
                        {
                            Path = moduleScript,
                            WebPath = normalizer.Normalize(moduleScript)
                        });
                    }

                    if (File.Exists(moduleVendor))
                    {
                        _moduleScripts.Add(new Script
                        {
                            Path = moduleVendor,
                            WebPath = normalizer.Normalize(moduleScript),
                            IsVendor = true
                        });
                    }
                }
            }
        }

        private void AddScriptFiles(string[] targetPathCollection)
        {
            foreach (var targetPath in targetPathCollection)
            {
                if (Directory.Exists(targetPath))
                {
                    var moduleScript = Path.Join(targetPath, "app.js");
                    var moduleVendor = Path.Join(targetPath, "vendor.js");

                    if (File.Exists(moduleScript))
                    {
                        _moduleScripts.Add(new Script
                        {
                            Path = moduleScript
                        });
                    }

                    if (File.Exists(moduleVendor))
                    {
                        _moduleScripts.Add(new Script
                        {
                            Path = moduleVendor,
                            IsVendor = true
                        });
                    }
                }
            }
        }

        private void AddFileVersion()
        {
            foreach (var moduleScript in _moduleScripts)
            {
                moduleScript.Version = _fileVersionProvider.GetFileVersion(moduleScript.Path);
            }
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

        private class Script
        {
            public string Path { get; set; }

            public string WebPath { get; set; }

            public string Version { get; set; }

            public bool IsVendor { get; set; }
        }
    }
}

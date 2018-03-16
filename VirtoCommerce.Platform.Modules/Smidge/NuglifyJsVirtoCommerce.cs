using System;
using System.IO;
using System.Linq;
using Smidge.CompositeFiles;
using System.Threading.Tasks;
using NUglify;
using Smidge.FileProcessors;
using Smidge.Models;
using Smidge.Nuglify;

namespace VirtoCommerce.Platform.Modules.Smidge
{
    public class NuglifyJsVirtoCommerce : IPreProcessor
    {
        private readonly NuglifySettings _settings;
        private readonly ISourceMapDeclaration _sourceMapDeclaration;

        public NuglifyJsVirtoCommerce(NuglifySettings settings, ISourceMapDeclaration sourceMapDeclaration)
        {
            _settings = settings;
            _sourceMapDeclaration = sourceMapDeclaration;
        }

        public Task ProcessAsync(FileProcessContext fileProcessContext, PreProcessorDelegate next)
        {

            //Info for source mapping, see http://ajaxmin.codeplex.com/wikipage?title=SourceMaps
            // as an example, see http://ajaxmin.codeplex.com/SourceControl/latest#AjaxMinTask/AjaxMinManifestTask.cs under ProcessJavaScript
            // When a source map provider is specified, the process is:
            // * Create a string builder/writer to capture the output of the source map
            // * Create a sourcemap from the SourceMapFactory based on the provider name
            // * Set some V3 source map provider (the default) properties
            // * Call StartPackage, passing in the file path of the original file and the file path of the map (which will be appended to the end of the minified output)
            // * Do the minification
            // * Call EndPackage, and close/dispose of writers
            // * Get the source map result from the string builder

            if (fileProcessContext.WebFile.DependencyType == WebFileType.Css)
                throw new InvalidOperationException("Cannot use " + nameof(NuglifyJs) + " with a css file source");

            var nuglifyJsCodeSettings = _settings.JsCodeSettings;

            //Its very important that we clone here because the code settings is a singleton and we are changing it (i.e. the CodeSettings class is mutable)
            var codeSettings = nuglifyJsCodeSettings.CodeSettings.Clone();
            var file = (JavaScriptFileVirtoCommerce)fileProcessContext.WebFile;

            if (nuglifyJsCodeSettings.SourceMapType != SourceMapType.None)
            {
                var sourceMap =
                    fileProcessContext.BundleContext.GetSourceMapFromContext(nuglifyJsCodeSettings.SourceMapType);

                codeSettings.SymbolsMap = sourceMap;

                //These are a couple of options that could be needed for V3 source maps

                //sourceRoot is explained here: 
                //  http://blog.teamtreehouse.com/introduction-source-maps
                //  https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/
                sourceMap.SourceRoot = "/sb/maps";

                //SafeHeader is used to avoid XSS and adds some custom json to the top of the file , here's what the source code says:
                // "if we want to add the cross-site script injection protection string" it adds this to the top ")]}'"
                // explained here: https://www.html5rocks.com/en/tutorials/developertools/sourcemaps/ under "Potential XSSI issues"
                // ** not needed for inline
                //sourceMap.SafeHeader = 

                //generate a minified file path - this is really not used but is used in our inline source map like test.js --> test.min.js

                //var extension = Path.GetExtension(file.RequestPath);
                //var minPath = file.RequestPath.Substring(
                //                  0,
                //                  file.RequestPath.LastIndexOf('.')) + extension;
                var extension = Path.GetExtension(file.FilePath);
                var minPath = file.FilePath.Substring(
                                  0,
                                  file.FilePath.LastIndexOf('.')) + extension;

                //we then need to 'StartPackage', this will be called once per file for the same source map instance but that is ok it doesn't cause any harm
                //var fileName = fileProcessContext.BundleContext.BundleName + fileProcessContext.BundleContext.FileExtension;
                var fileName = minPath;
                sourceMap.StartPackage(fileName, fileName + ".map");
            }

            //no do the processing
            var result = Uglify.Js(fileProcessContext.FileContent, file.FilePath, codeSettings);

            if (result.HasErrors)
            {
                //TODO: need to format this exception message nicely
                throw new InvalidOperationException(
                    string.Join(",", result.Errors.Select(x => x.Message)));
            }

            fileProcessContext.Update(result.Code);

            if (nuglifyJsCodeSettings.SourceMapType != SourceMapType.None)
            {
                AddSourceMapAppenderToContext(fileProcessContext.BundleContext, nuglifyJsCodeSettings.SourceMapType);
            }

            return next(fileProcessContext);
        }


        /// <summary>
        /// Adds a SourceMapAppender into the current bundle context if it doesn't already exist
        /// </summary>
        /// <param name="bundleContext"></param>
        /// <param name="sourceMapType"></param>
        /// <returns></returns>
        private void AddSourceMapAppenderToContext(BundleContext bundleContext, SourceMapType sourceMapType)
        {
            //if it already exist, then ignore
            var key = "SourceMapDeclaration";
            if (bundleContext.Items.TryGetValue(key, out object sm))
            {
                return;
            }

            //not in the context so add a flag so it's not re-added
            bundleContext.Items[key] = "added";

            bundleContext.AddAppender(() =>
            {
                var sourceMap = bundleContext.GetSourceMapFromContext(sourceMapType);
                return _sourceMapDeclaration.GetDeclaration(bundleContext, sourceMap);
            });
        }
    }
}

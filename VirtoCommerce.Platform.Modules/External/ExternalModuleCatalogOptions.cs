using System;

namespace VirtoCommerce.Platform.Modules
{
    public class ExternalModuleCatalogOptions
    {
        public Uri ModulesManifestUrl { get; set; }
        public string AuthorizationToken { get; set; }
        public string[] AutoInstallModuleBundles { get; set; } = new[] { "commerce" };
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Modules
{
    public class ExternalModuleCatalogOptions
    {
        public Uri ModulesManifestUrl { get; set; }
        public string AuthorizationToken { get; set; }
    }
}

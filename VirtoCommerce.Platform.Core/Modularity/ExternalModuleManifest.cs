using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Modularity
{
    // Represents the module information with all of it historical versions and is used to download from an external source for installation and update operations.
    public class ExternalModuleManifest : ValueObject
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Authors { get; set; }
        public string[] Owners { get; set; }

        public string LicenseUrl { get; set; }

        public string ProjectUrl { get; set; }

        public string IconUrl { get; set; }
        public bool RequireLicenseAcceptance { get; set; }
 
        public string Copyright { get; set; }

        public string Tags { get; set; }

        public string[] Groups { get; set; }

        public ExternalModuleManifestVersion[] Versions { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}

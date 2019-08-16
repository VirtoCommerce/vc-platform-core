using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public class ExternalModuleManifestVersion : ValueObject
    {
        public string Version { get; set; }
        public SemanticVersion SemanticVersion => SemanticVersion.Parse(Version);

        public string PlatformVersion { get; set; }
        public SemanticVersion PlatformSemanticVersion => SemanticVersion.Parse(PlatformVersion);

        public string PackageUrl { get; set; }

        public ManifestDependency[] Incompatibilities { get; set; }
        public ManifestDependency[] Dependencies { get; set; }

        public string ReleaseNotes { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;
        }
    }
}

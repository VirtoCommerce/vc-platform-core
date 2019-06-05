using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public class ModuleIdentity : ValueObject
    {
        public ModuleIdentity(string id, SemanticVersion version)
        {
            Id = id;
            Version = version;
        }
        public ModuleIdentity(string id, System.Version version)
              : this(id, new SemanticVersion(version))
        {
        }
        public ModuleIdentity(string id, string version)
            : this(id, new System.Version(version))
        {
        }

        public string Id { get; private set; }
        public SemanticVersion Version { get; private set; }

        public override string ToString()
        {
            return $"{Id}:{Version.ToString()}";
        }
    }
}

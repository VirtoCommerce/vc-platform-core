using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeDefinition
    {
        public string TypeName { get; set; }
        public ExportedTypeMetadata[] MetaData { get; set; }
    }
}

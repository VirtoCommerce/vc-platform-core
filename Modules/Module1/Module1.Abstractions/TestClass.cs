using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace Module1.Abstractions
{
    public class TestClass : Entity, IHasDynamicProperties
    {
        public string ObjectType => GetType().FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

    }
}

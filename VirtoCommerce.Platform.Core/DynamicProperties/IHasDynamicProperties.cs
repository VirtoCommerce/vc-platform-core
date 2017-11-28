using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IHasDynamicProperties : IEntity
    {
        string ObjectType { get; set; }
        ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
    }
}

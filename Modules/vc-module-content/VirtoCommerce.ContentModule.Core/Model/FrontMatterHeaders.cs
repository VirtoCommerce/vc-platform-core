using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ContentModule.Web.Model
{
    public class FrontMatterHeaders : Entity, IHasDynamicProperties
    {
        public string ObjectType => GetType().FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
    }
}

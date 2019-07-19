using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public partial class ViewableEntity : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
    }
}

using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public partial class ViewableEntity : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public virtual void FromEntity(Entity entity)
        {
            Id = entity.Id;
            Type = entity.GetType().Name;
        }
    }
}

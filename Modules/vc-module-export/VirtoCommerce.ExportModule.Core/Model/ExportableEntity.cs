using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportableEntity<T> : Entity, IExportViewable where T : Entity
    {
        #region IExportViewable implementation
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }
        #endregion

        public object Clone()
        {
            return MemberwiseClone() as T;
        }
    }
}

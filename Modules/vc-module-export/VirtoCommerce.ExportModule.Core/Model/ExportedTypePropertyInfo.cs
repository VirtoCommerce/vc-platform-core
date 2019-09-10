namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Export property information
    /// </summary>
    public class ExportedTypePropertyInfo
    {
        /// <summary>
        /// Property name with the path from the exportable entity (e.g. for entity containing PropertyA with nested properties it could be "PropertyA.PropertyB.PropertyC").
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Property group. Properties can be divided into different groups to simplify selection.
        /// Group could be used for grouping property infos.
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// User-friendly name for this property
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// * Reserved for future use
        /// </summary>
        public bool IsRequired { get; set; }
    }
}

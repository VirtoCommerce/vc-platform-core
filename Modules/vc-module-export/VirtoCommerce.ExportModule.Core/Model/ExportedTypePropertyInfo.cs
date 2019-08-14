namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Export property information
    /// </summary>
    public class ExportedTypePropertyInfo
    {
        /// <summary>
        /// Full property name of the exportable entity (i.e. "PropertyA.PropertyB.PropertyC").
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Property group. Properties can be divided into different groups to simplify selection by user on the properties selector blade.
        /// Properties within a group come together on the blade.
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

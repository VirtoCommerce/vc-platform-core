namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Information for single exported property
    /// </summary>
    public class ExportedTypePropertyInfo
    {
        /// <summary>
        /// Property name (with path) of the exportable entity.
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Property group. Properties can be divided into different groups to simplify selection by user on the properties selector blade.
        /// Properties with the same Group fall into one group.
        /// </summary>
        public string Group { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
    }
}

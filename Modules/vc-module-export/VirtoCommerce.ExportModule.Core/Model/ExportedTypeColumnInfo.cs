namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Information for single exported column
    /// </summary>
    public class ExportedTypeColumnInfo
    {
        /// <summary>
        /// Column name. This is the same as a property name (with path) of the <see cref="ExportableEntity{T}"/> descendant.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Column grouping. Columns can be divided into different groups to simplify their selection by user on the columns selector blade.
        /// Columns with the same Group fall into one group.
        /// </summary>
        public string Group { get; set; }
        public string ExportName { get; set; }
        public bool IsRequired { get; set; }
    }
}

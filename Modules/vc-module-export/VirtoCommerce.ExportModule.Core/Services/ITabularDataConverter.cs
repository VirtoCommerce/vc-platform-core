namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface to convert exportable entity to tabular representation
    /// </summary>
    public interface ITabularDataConverter
    {
        /// <summary>
        /// Converts exportable to tabular representation
        /// </summary>
        /// <param name="obj">exportable entity</param>
        /// <returns>tabular representation</returns>
        object ToTabular(object obj);
    }
}

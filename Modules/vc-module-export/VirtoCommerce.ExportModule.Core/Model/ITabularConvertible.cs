namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Provides conversion to tabular representation. 
    /// </summary>
    public interface ITabularConvertible
    {
        /// <summary>
        /// Converts to tabular representation.
        /// </summary>
        /// <returns></returns>
        IExportable ToTabular();
    }
}

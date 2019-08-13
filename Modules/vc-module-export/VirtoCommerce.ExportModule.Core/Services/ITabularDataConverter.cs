namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface to convert ExportableEntity to tabular entity form (without included objects)
    /// </summary>
    public interface ITabularDataConverter
    {
        /// <summary>
        /// Convert ExportableEntity to tabular entity form
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object ToTabular(object obj);
    }
}

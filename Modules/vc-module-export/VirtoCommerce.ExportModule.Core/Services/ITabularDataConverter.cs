using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface to convert exportable entity to tabular representation.
    /// </summary>
    public interface ITabularDataConverter
    {
        /// <summary>
        /// Converts exportable entity to tabular representation.
        /// </summary>
        /// <param name="obj">Exportable entity.</param>
        /// <returns>Tabular (plain) representation.</returns>
        IExportable ToTabular(IExportable obj);
    }
}

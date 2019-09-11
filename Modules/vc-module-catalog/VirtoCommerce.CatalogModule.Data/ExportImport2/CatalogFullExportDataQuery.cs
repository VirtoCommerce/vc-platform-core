using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportDataQuery : ExportDataQuery
    {
        public string[] CatalogIds { get; set; }

        public virtual CatalogFullExportDataQuery FromOther(CatalogFullExportDataQuery other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            CatalogIds = other.CatalogIds;
            return this;
        }
    }
}

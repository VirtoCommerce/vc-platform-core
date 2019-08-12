using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceExportDataQuery : ExportDataQuery
    {
        public string[] PriceListIds { get; set; }

        public string[] ProductIds { get; set; }

        public DateTime? ModifiedSince { get; set; }
    }
}

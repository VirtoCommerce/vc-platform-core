using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
    public class TaxEvaluationContext : Entity, IEvaluationContext
    {
        public TaxEvaluationContext()
        {
            Lines = new List<TaxLine>();
        }

        public string StoreId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        public string CustomerId { get; set; }
        public string OrganizationId { get; set; }
        public Address Address { get; set; }
        public string Currency { get; set; }
        public ICollection<TaxLine> Lines { get; set; }

    }
}

using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
    public class TaxEvaluationContext : Entity, IEvaluationContext
    {
        public TaxEvaluationContext()
        {
            Lines = new List<TaxLine>();
        }

        //TODO
        //public VirtoCommerce.Domain.Store.Model.Store Store { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        //TODO
        //public Customer.Model.Contact Customer { get; set; }
        //public Customer.Model.Organization Organization { get; set; }
        public Address Address { get; set; }
        public string Currency { get; set; }
        public ICollection<TaxLine> Lines { get; set; }

    }
}

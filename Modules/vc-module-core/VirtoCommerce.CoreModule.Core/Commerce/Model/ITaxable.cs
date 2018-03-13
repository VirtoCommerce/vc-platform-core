using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Domain.Commerce.Model
{
    public interface ITaxable
    {
        /// <summary>
        /// Tax category or type
        /// </summary>
        string TaxType { get; set; }

        decimal TaxTotal { get; }
        decimal TaxPercentRate { get; }
    }
}

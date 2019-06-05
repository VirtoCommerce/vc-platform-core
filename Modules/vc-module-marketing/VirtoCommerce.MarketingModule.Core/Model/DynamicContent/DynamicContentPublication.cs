using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPublication : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public string StoreId { get; set; }

        public string PredicateVisualTreeSerialized { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ICollection<DynamicContentItem> ContentItems { get; set; }
        public ICollection<DynamicContentPlace> ContentPlaces { get; set; }

        /// <summary>
        /// Dynamic conditions tree determine the applicability of this publication
        /// </summary>
        public IConditionTree DynamicExpression { get; set; }
    }
}

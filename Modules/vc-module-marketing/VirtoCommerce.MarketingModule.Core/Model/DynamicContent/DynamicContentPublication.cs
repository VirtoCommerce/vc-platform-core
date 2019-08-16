using System;
using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
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

        public DynamicContentConditionTree DynamicExpression { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string OuterId { get; set; }

        public ICollection<DynamicContentItem> ContentItems { get; set; }
        public ICollection<DynamicContentPlace> ContentPlaces { get; set; }
       
    }
}

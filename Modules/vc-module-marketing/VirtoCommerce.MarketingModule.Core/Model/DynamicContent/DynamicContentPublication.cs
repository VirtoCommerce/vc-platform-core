using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPublication : AuditableEntity, ICloneable
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

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as DynamicContentPublication;

            if (DynamicExpression != null)
            {
                result.DynamicExpression = DynamicExpression.Clone() as DynamicContentConditionTree;
            }

            if (ContentItems != null)
            {
                result.ContentItems = new ObservableCollection<DynamicContentItem>(
                    ContentItems.Select(x => x.Clone() as DynamicContentItem));
            }

            if (ContentPlaces != null)
            {
                result.ContentPlaces = new ObservableCollection<DynamicContentPlace>(
                    ContentPlaces.Select(x => x.Clone() as DynamicContentPlace));
            }

            return result;
        }

        #endregion

    }
}

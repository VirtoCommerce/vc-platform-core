using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Search
{
    public class CouponSearchCriteria : SearchCriteriaBase
    {
        public string Code { get; set; }

        private ICollection<string> _couponCodes;
        public ICollection<string> Codes
        {
            get
            {
                if (_couponCodes == null && !string.IsNullOrEmpty(Code))
                {
                    _couponCodes = new List<string>() { Code };
                }
                return _couponCodes;
            }
            set
            {
                _couponCodes = value;
            }
        }
        public string PromotionId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    /// <summary>
    /// Uses for extensions of promotions and dynamic content dynamic expressions
    /// </summary>
	public class DefaultMarketingExtensionManager : IMarketingExtensionManager
    {
        private List<Promotion> _promotions = new List<Promotion>();

        #region InMemoryExtensionManagerImpl Members

        public IConditionTree PromotionCondition { get; set; }
        public IConditionTree ContentCondition { get; set; }

        public void RegisterPromotion(Promotion promotion)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion));
            }
            if (_promotions.Any(x => x.Id == promotion.Id))
            {
                throw new OperationCanceledException($"{promotion.Id} already registered");
            }
            _promotions.Add(promotion);
        }

        public IEnumerable<Promotion> Promotions => _promotions.AsReadOnly();

        #endregion
    }
}

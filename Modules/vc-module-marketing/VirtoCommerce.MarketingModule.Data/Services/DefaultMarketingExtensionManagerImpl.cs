using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    /// <summary>
    /// Uses for extensions of promotions and dynamic content dynamic expressions
    /// </summary>
	public class DefaultMarketingExtensionManagerImpl : IMarketingExtensionManager
    {
        private List<Promotion> _promotions = new List<Promotion>();

        #region InMemoryExtensionManagerImpl Members

        //TODO
        public IConditionRewardTree PromotionCondition { get; set; }
        public IConditionRewardTree ContentCondition { get; set; }

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

        public IEnumerable<Promotion> Promotions
        {
            get { return _promotions.AsReadOnly(); }
        }

        #endregion
    }
}

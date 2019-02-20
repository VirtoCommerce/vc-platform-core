using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{

    public class PromotionResult
    {
        public ICollection<PromotionReward> Rewards { get; } = new List<PromotionReward>();

        public T GetReward<T>() where T : PromotionReward
        {
            return Rewards.OfType<T>().FirstOrDefault(x => x.IsValid);
        }

        public T GetPotentialReward<T>() where T : PromotionReward
        {
            return Rewards.OfType<T>().FirstOrDefault(x => !x.IsValid);
        }

        public T[] GetRewards<T>() where T : PromotionReward
        {
            return Rewards.OfType<T>().Where(x => x.IsValid).ToArray();
        }

        public T[] GetPotentialRewards<T>() where T : PromotionReward
        {
            return Rewards.OfType<T>().Where(x => !x.IsValid).ToArray();
        }
    }
}

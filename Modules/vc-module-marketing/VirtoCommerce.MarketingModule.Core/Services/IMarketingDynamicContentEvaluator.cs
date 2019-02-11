using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingDynamicContentEvaluator
    {
        Task<DynamicContentItem[]> EvaluateItemsAsync(IEvaluationContext context);
    }
}

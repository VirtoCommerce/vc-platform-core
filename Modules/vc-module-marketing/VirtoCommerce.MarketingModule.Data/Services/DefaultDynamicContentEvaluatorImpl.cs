using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Services;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class DefaultDynamicContentEvaluatorImpl : IMarketingDynamicContentEvaluator
    {
        private readonly IDynamicContentService _dynamicContentService;
        private readonly ILogger _logger;

        public DefaultDynamicContentEvaluatorImpl(IDynamicContentService dynamicContentService, ILogger<DefaultDynamicContentEvaluatorImpl> logger)
        {
            _dynamicContentService = dynamicContentService;
            _logger = logger;
        }

        #region IMarketingDynamicContentEvaluator Members

        public async Task<DynamicContentItem[]> EvaluateItemsAsync(IEvaluationContext context)
        {
            if (!(context is DynamicContentEvaluationContext dynamicContext))
            {
                throw new ArgumentException("The context must be a DynamicContentEvaluationContext.");
            }
            if (dynamicContext.ToDate == default(DateTime))
            {
                dynamicContext.ToDate = DateTime.UtcNow;
            }
            var retVal = new List<DynamicContentItem>();

            var publishings = await _dynamicContentService.GetContentPublicationsByStoreIdAndPlaceNameAsync(dynamicContext.StoreId, dynamicContext.ToDate, dynamicContext.PlaceName);

            var contentItemIds = new List<string>();
            foreach (var publishing in publishings.Where(x => x.DynamicExpression != null))
            {
                try
                {
                    //Next step need filter assignments contains dynamicexpression
                    if (!string.IsNullOrEmpty(publishing.PredicateVisualTreeSerialized))
                    {
                        var dynamicContentConditionTree = JsonConvert.DeserializeObject<DynamicContentConditionTree>(publishing.PredicateVisualTreeSerialized, new ConditionJsonConverter());
                        var conditions = dynamicContentConditionTree.GetConditions();
                        if (conditions.All(c => c.Evaluate(context)))
                        {
                            contentItemIds.AddRange(publishing.ContentItems.Select(x => x.Id));
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }

            var dynamicContentItems = await _dynamicContentService.GetContentItemsByIdsAsync(contentItemIds.ToArray());
            retVal.AddRange(dynamicContentItems);

            return retVal.ToArray();
        }

        #endregion
    }
}

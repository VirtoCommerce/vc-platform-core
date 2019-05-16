using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Repositories;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class DefaultDynamicContentEvaluatorImpl : IMarketingDynamicContentEvaluator
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly ILogger _logger;

        public DefaultDynamicContentEvaluatorImpl(Func<IMarketingRepository> repositoryFactory, IDynamicContentService dynamicContentService, ILogger<DefaultDynamicContentEvaluatorImpl> logger)
        {
            _repositoryFactory = repositoryFactory;
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
            using (var repository = _repositoryFactory())
            {
                var publishingEntities = await repository.PublishingGroups
                                                       .Include(x => x.ContentItems)
                                                       .Where(x => x.IsActive)
                                                       .Where(x => x.StoreId == dynamicContext.StoreId)
                                                       .Where(x => (x.StartDate == null || dynamicContext.ToDate >= x.StartDate) && (x.EndDate == null || x.EndDate >= dynamicContext.ToDate))
                                                       .Where(x => x.ContentPlaces.Any(y => y.ContentPlace.Name == dynamicContext.PlaceName))
                                                       .OrderBy(x => x.Priority)
                                                       .ToArrayAsync();

                var contentItemIds = new List<string>();
                foreach (var publishing in publishingEntities.Where(x => x.ConditionExpression != null))
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
                                contentItemIds.AddRange(publishing.ContentItems.Select(x => x.DynamicContentItemId));
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
            }

            return retVal.ToArray();
        }

        #endregion
    }
}

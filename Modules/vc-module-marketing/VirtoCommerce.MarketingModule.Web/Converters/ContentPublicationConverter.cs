using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class DynamicContentPublicationConverter
    {
        public static webModel.DynamicContentPublication ToWebModel(this coreModel.DynamicContentPublication publication, IConditionTree etalonEpressionTree = null)
        {
            var retVal = new webModel.DynamicContentPublication()
            {
                Id = publication.Id,
                CreatedDate = publication.CreatedDate,
                CreatedBy = publication.CreatedBy,
                ModifiedDate = publication.ModifiedDate,
                ModifiedBy = publication.ModifiedBy,
                Description = publication.Description,
                Name = publication.Name,
                Priority = publication.Priority,
                IsActive = publication.IsActive,
                StoreId = publication.StoreId,
                StartDate = publication.StartDate,
                EndDate = publication.EndDate
            };

            if (publication.ContentItems != null)
            {
                retVal.ContentItems = publication.ContentItems.Select(x => x.ToWebModel()).ToList();
            }
            if (publication.ContentPlaces != null)
            {
                retVal.ContentPlaces = publication.ContentPlaces.Select(x => x.ToWebModel()).ToList();
            }

            retVal.DynamicExpression = etalonEpressionTree;
            if (!string.IsNullOrEmpty(publication.PredicateVisualTreeSerialized))
            {
                retVal.DynamicExpression = JsonConvert.DeserializeObject<IConditionTree>(publication.PredicateVisualTreeSerialized, new ConditionRewardJsonConverter());
                if (etalonEpressionTree != null)
                {
                    //Copy available elements from etalon because they not persisted
                    var sourceBlocks = etalonEpressionTree.Traverse(x => x.Children);
                    var targetBlocks = retVal.DynamicExpression.Traverse(x => x.Children).ToList();
                    foreach (var sourceBlock in sourceBlocks)
                    {
                        foreach (var targetBlock in targetBlocks.Where(x => x.Id == sourceBlock.Id))
                        {
                            targetBlock.AvailableChildren = sourceBlock.AvailableChildren;
                        }
                    }
                    //copy available elements from etalon
                    retVal.DynamicExpression.AvailableChildren = etalonEpressionTree.AvailableChildren;
                }
            }

            return retVal;
        }

        public static coreModel.DynamicContentPublication ToCoreModel(this webModel.DynamicContentPublication publication)
        {
            var retVal = new coreModel.DynamicContentPublication
            {
                Id = publication.Id,
                CreatedDate = publication.CreatedDate,
                CreatedBy = publication.CreatedBy,
                ModifiedDate = publication.ModifiedDate,
                ModifiedBy = publication.ModifiedBy,
                Description = publication.Description,
                Name = publication.Name,
                Priority = publication.Priority,
                IsActive = publication.IsActive,
                StoreId = publication.StoreId,
                StartDate = publication.StartDate,
                EndDate = publication.EndDate
            };

            if (publication.ContentItems != null)
            {
                retVal.ContentItems = publication.ContentItems.Select(x => x.ToCoreModel()).ToList();
            }
            if (publication.ContentPlaces != null)
            {
                retVal.ContentPlaces = publication.ContentPlaces.Select(x => x.ToCoreModel()).ToList();
            }

            if (publication.DynamicExpression != null)
            {
                var conditionExpression = ((ICondition)publication.DynamicExpression).GetConditions();
                retVal.PredicateSerialized = JsonConvert.SerializeObject(conditionExpression);

                //Clear availableElements in expression (for decrease size)
                publication.DynamicExpression.AvailableChildren = null;
                var allBlocks = publication.DynamicExpression.Traverse(x => x.Children);
                foreach (var block in allBlocks)
                {
                    block.AvailableChildren = null;
                }
                retVal.PredicateVisualTreeSerialized = JsonConvert.SerializeObject(publication.DynamicExpression);
            }

            return retVal;
        }
    }
}

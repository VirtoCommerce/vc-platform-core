using System.Linq;
using System.Linq.Expressions;
using VirtoCommerce.Platform.Core.Serialization;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class DynamicContentPublicationConverter
    {
        public static webModel.DynamicContentPublication ToWebModel(this coreModel.DynamicContentPublication publication, Expression etalonEpressionTree = null)
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

            //TODO
            //retVal.DynamicExpression = etalonEpressionTree;
            //if (!string.IsNullOrEmpty(publication.PredicateVisualTreeSerialized))
            //{
            //    retVal.DynamicExpression = JsonConvert.DeserializeObject<ConditionExpressionTree>(publication.PredicateVisualTreeSerialized);
            //    if (etalonEpressionTree != null)
            //    {
            //        //Copy available elements from etalon because they not persisted
            //        var sourceBlocks = ((DynamicExpression)etalonEpressionTree).Traverse(x => x.Children);
            //        var targetBlocks = ((DynamicExpression)retVal.DynamicExpression).Traverse(x => x.Children).ToList();
            //        foreach (var sourceBlock in sourceBlocks)
            //        {
            //            foreach (var targetBlock in targetBlocks.Where(x => x.Id == sourceBlock.Id))
            //            {
            //                targetBlock.AvailableChildren = sourceBlock.AvailableChildren;
            //            }
            //        }
            //        //copy available elements from etalon
            //        retVal.DynamicExpression.AvailableChildren = etalonEpressionTree.AvailableChildren;
            //    }
            //}
            return retVal;
        }

        public static coreModel.DynamicContentPublication ToCoreModel(this webModel.DynamicContentPublication publication, IExpressionSerializer expressionSerializer)
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

            //TODO
            //if (publication.DynamicExpression != null)
            //{
            //    var conditionExpression = publication.DynamicExpression.GetConditionExpression();
            //    retVal.PredicateSerialized = expressionSerializer.SerializeExpression(conditionExpression);

            //    //Clear availableElements in expression (for decrease size)
            //    publication.DynamicExpression.AvailableChildren = null;
            //    var allBlocks = ((DynamicExpression)publication.DynamicExpression).Traverse(x => x.Children);
            //    foreach (var block in allBlocks)
            //    {
            //        block.AvailableChildren = null;
            //    }
            //    retVal.PredicateVisualTreeSerialized = JsonConvert.SerializeObject(publication.DynamicExpression);
            //}

            return retVal;
        }
    }
}

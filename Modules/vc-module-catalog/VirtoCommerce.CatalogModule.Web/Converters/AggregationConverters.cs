using System.Linq;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class AggregationConverters
    {
        public static webModel.Aggregation ToWebModel(this coreModel.Search.Aggregation aggregation)
        {
            var result = new webModel.Aggregation
            {
                AggregationType = aggregation.AggregationType,
                Field = aggregation.Field,
            };

            if (aggregation.Items != null)
            {
                result.Items = aggregation.Items.Select(i => i.ToWebModel()).ToArray();
            }

            if (aggregation.Labels != null)
            {
                result.Labels = aggregation.Labels.Select(ToWebModel).ToArray();
            }

            return result;
        }

        public static webModel.AggregationItem ToWebModel(this coreModel.Search.AggregationItem item)
        {
            var result = new webModel.AggregationItem
            {
                Value = item.Value,
                Count = item.Count,
                IsApplied = item.IsApplied,
                RequestedLowerBound = item.RequestedLowerBound,
                RequestedUpperBound = item.RequestedUpperBound,
            };

            if (item.Labels != null)
            {
                result.Labels = item.Labels.Select(ToWebModel).ToArray();
            }

            return result;
        }

        public static webModel.AggregationLabel ToWebModel(this coreModel.Search.AggregationLabel label)
        {
            return new webModel.AggregationLabel
            {
                Language = label.Language,
                Label = label.Label,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Tests
{
    public abstract class SearchProviderTestsBase
    {
        protected abstract ISearchProvider GetSearchProvider();

        protected virtual IList<IndexDocument> GetPrimaryDocuments()
        {
            return new List<IndexDocument>
            {
                CreateDocument("Item-1", "Sample Product", "Red", "2017-04-28T15:24:31.180Z", 2, "0,0", null, null, new Price("USD", "default", 123.23m)),
                CreateDocument("Item-2", "Red Shirt 2", "Red", "2017-04-27T15:24:31.180Z", 4, "0,10", null, null, new Price("USD", "default", 200m), new Price("USD", "sale", 99m), new Price("EUR", "sale", 300m)),
                CreateDocument("Item-3", "Red Shirt", "Red", "2017-04-26T15:24:31.180Z", 3, "0,20", null, null, new Price("USD", "default", 10m)),
                CreateDocument("Item-4", "Black Sox", "Black", "2017-04-25T15:24:31.180Z", 10, "0,30", null, null, new Price("USD", "default", 243.12m), new Price("USD", "supersale", 89m)),
                CreateDocument("Item-5", "Black Sox2", "Silver", "2017-04-24T15:24:31.180Z", 20, "0,40", null, null, new Price("USD", "default", 700m)),
            };
        }

        protected virtual IList<IndexDocument> GetSecondaryDocuments()
        {
            return new List<IndexDocument>
            {
                CreateDocument("Item-6", "Blue Shirt", "Blue", "2017-04-23T15:24:31.180Z", 10, "0,50", "Blue Shirt 2", DateTime.UtcNow, new Price("USD", "default", 23.12m)),

                // The following documents will be deleted by test
                CreateDocument("Item-7", "Blue Shirt", "Blue", "2017-04-23T15:24:31.180Z", 10, "0,50", "Blue Shirt 2", DateTime.UtcNow, new Price("USD", "default", 23.12m)),
                CreateDocument("Item-8", "Blue Shirt", "Blue", "2017-04-23T15:24:31.180Z", 10, "0,50", "Blue Shirt 2", DateTime.UtcNow, new Price("USD", "default", 23.12m)),
            };
        }

        protected virtual IndexDocument CreateDocument(string id, string name, string color, string date, int size, string location, string name2, DateTime? date2, params Price[] prices)
        {
            var doc = new IndexDocument(id);

            doc.Add(new IndexDocumentField("Content", name) { IsRetrievable = true, IsSearchable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("Content", color) { IsRetrievable = true, IsSearchable = true, IsCollection = true });

            doc.Add(new IndexDocumentField("Code", id) { IsRetrievable = true, IsFilterable = true });
            doc.Add(new IndexDocumentField("Name", name) { IsRetrievable = true, IsFilterable = true });
            doc.Add(new IndexDocumentField("Color", color) { IsRetrievable = true, IsFilterable = true });
            doc.Add(new IndexDocumentField("Size", size) { IsRetrievable = true, IsFilterable = true });
            doc.Add(new IndexDocumentField("Date", DateTime.Parse(date)) { IsRetrievable = true, IsFilterable = true });
            doc.Add(new IndexDocumentField("Location", GeoPoint.TryParse(location)) { IsRetrievable = true, IsFilterable = true });

            doc.Add(new IndexDocumentField("Catalog", "Goods") { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("Catalog", "Stuff") { IsRetrievable = true, IsFilterable = true, IsCollection = true });

            doc.Add(new IndexDocumentField("NumericCollection", size) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("NumericCollection", 10) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("NumericCollection", 20) { IsRetrievable = true, IsFilterable = true, IsCollection = true });

            doc.Add(new IndexDocumentField("Is", "Priced") { IsFilterable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("Is", color) { IsFilterable = true, IsCollection = true });
            doc.Add(new IndexDocumentField("Is", id) { IsFilterable = true, IsCollection = true });

            doc.Add(new IndexDocumentField("StoredField", "This value should not be processed in any way, it is just stored in the index.") { IsRetrievable = true });

            foreach (var price in prices)
            {
                doc.Add(new IndexDocumentField($"Price_{price.Currency}_{price.Pricelist}", price.Amount) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                doc.Add(new IndexDocumentField($"Price_{price.Currency}", price.Amount) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
            }

            var hasMultiplePrices = prices.Length > 1;
            doc.Add(new IndexDocumentField("HasMultiplePrices", hasMultiplePrices) { IsRetrievable = true, IsFilterable = true });

            // Adds extra fields to test mapping updates for indexer
            if (name2 != null)
            {
                doc.Add(new IndexDocumentField("Name 2", name2) { IsRetrievable = true, IsFilterable = true });
            }

            if (date2 != null)
            {
                doc.Add(new IndexDocumentField("Date (2)", date2) { IsRetrievable = true, IsFilterable = true });
            }

            return doc;
        }

        protected virtual ISettingsManager GetSettingsManager()
        {
            var mock = new Mock<ITestSettingsManager>();

            mock.Setup(s => s.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns((string name, string defaultValue) => defaultValue);
            mock.Setup(s => s.GetValue(It.IsAny<string>(), It.IsAny<bool>())).Returns((string name, bool defaultValue) => defaultValue);
            mock.Setup(s => s.GetValue(It.IsAny<string>(), It.IsAny<int>())).Returns((string name, int defaultValue) => defaultValue);
            mock.Setup(s => s.GetObjectSettingAsync(It.IsAny<string>(), null, null))
                .Returns(Task.FromResult(new ObjectSettingEntry()));

            return mock.Object;
        }

        protected virtual IFilter CreateRangeFilter(string fieldName, string lower, string upper, bool includeLower, bool includeUpper)
        {
            return new RangeFilter
            {
                FieldName = fieldName,
                Values = new[]
                {
                    new RangeFilterValue
                    {
                        Lower = lower,
                        Upper = upper,
                        IncludeLower = includeLower,
                        IncludeUpper = includeUpper,
                    }
                },
            };
        }

        protected virtual long GetAggregationValuesCount(SearchResponse response, string aggregationId)
        {
            var aggregation = GetAggregation(response, aggregationId);
            var result = aggregation?.Values?.Count ?? 0;
            return result;
        }

        protected virtual long GetAggregationValueCount(SearchResponse response, string aggregationId, string valueId)
        {
            var aggregation = GetAggregation(response, aggregationId);
            var result = GetAggregationValueCount(aggregation, valueId);
            return result;
        }

        protected virtual AggregationResponse GetAggregation(SearchResponse response, string aggregationId)
        {
            AggregationResponse result = null;

            if (response?.Aggregations?.Count > 0)
            {
                result = response.Aggregations.SingleOrDefault(a => a.Id.EqualsInvariant(aggregationId));
            }

            return result;
        }

        protected virtual long GetAggregationValueCount(AggregationResponse aggregation, string valueId)
        {
            long? result = null;

            if (aggregation?.Values?.Count > 0)
            {
                result = aggregation.Values
                    .Where(v => v.Id == valueId)
                    .Select(facet => facet.Count)
                    .SingleOrDefault();
            }

            return result ?? 0;
        }

        protected class Price
        {
            public Price(string currency, string pricelist, decimal amount)
            {
                Currency = currency;
                Pricelist = pricelist;
                Amount = amount;
            }

            public string Currency;
            public string Pricelist;
            public decimal Amount;
        }

        /// <summary>
        /// Allowing to moq extensions methods
        /// </summary>
        public interface ITestSettingsManager : ISettingsManager
        {
            T GetValue<T>(string name, T defaultValue);
            Task<T> GetValueAsync<T>(string name, T defaultValue);
        }
    }
}

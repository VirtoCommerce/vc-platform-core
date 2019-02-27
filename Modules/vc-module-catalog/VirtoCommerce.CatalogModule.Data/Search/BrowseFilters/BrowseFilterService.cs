using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.CatalogModule.Data.Search.BrowseFilters
{
    public class BrowseFilterService : IBrowseFilterService
    {
        public const string FilteredBrowsingPropertyName = "FilteredBrowsing";

        private readonly IStoreService _storeService;

        public BrowseFilterService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(FilteredBrowsing));
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer
        {
            DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
        };

        public IList<IBrowseFilter> GetBrowseFilters(ProductSearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var aggregations = GetAllAggregations(criteria)?.AsQueryable();

            // Check allowed aggregations
            if (criteria.IncludeAggregations != null)
            {
                aggregations = aggregations?.Where(f => criteria.IncludeAggregations.Contains(f.Key, StringComparer.OrdinalIgnoreCase));
            }

            // Check forbidden aggregations
            if (criteria.ExcludeAggregations != null)
            {
                aggregations = aggregations?.Where(f => !criteria.ExcludeAggregations.Contains(f.Key, StringComparer.OrdinalIgnoreCase));
            }

            var result = aggregations
                ?.Where(f => !(f is PriceRangeFilter) || ((PriceRangeFilter)f).Currency.EqualsInvariant(criteria.Currency))
                .ToList();

            return result;
        }

        public virtual IList<IBrowseFilter> GetStoreAggregations(string storeId)
        {
            var serializedValue = GetSerializedValue(storeId);
            var result = Deserialize(serializedValue);
            return result;
        }

        public virtual void SaveStoreAggregations(string storeId, IList<IBrowseFilter> filters)
        {
            var serializedValue = Serialize(filters);
            SaveSerializedValue(storeId, serializedValue);
        }


        protected virtual IList<IBrowseFilter> GetAllAggregations(ProductSearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return GetStoreAggregations(criteria.StoreId);
        }

        protected virtual string GetSerializedValue(string storeId)
        {
            var store = _storeService.GetById(storeId);
            var result = store?.GetDynamicPropertyValue(FilteredBrowsingPropertyName, string.Empty);
            return result;
        }

        protected virtual void SaveSerializedValue(string storeId, string serializedValue)
        {
            var store = _storeService.GetById(storeId);
            if (store != null)
            {
                var property = store.DynamicProperties.FirstOrDefault(p => p.Name == FilteredBrowsingPropertyName);
                if (property == null)
                {
                    property = new DynamicObjectProperty { Name = FilteredBrowsingPropertyName };
                    store.DynamicProperties.Add(property);
                }

                property.Values = new List<DynamicPropertyObjectValue>(new[] { new DynamicPropertyObjectValue { Value = serializedValue } });

                _storeService.Update(new[] { store });
            }
        }


        // Support JSON for serialization
        private static string Serialize(IList<IBrowseFilter> filters)
        {
            string result = null;

            if (filters != null)
            {
                // Group by type
                var browsing = new FilteredBrowsing
                {
                    Attributes = filters.OfType<AttributeFilter>().ToArray(),
                    AttributeRanges = filters.OfType<RangeFilter>().ToArray(),
                    Prices = filters.OfType<PriceRangeFilter>().ToArray(),
                };

                // Serialize to JSON
                using (var memStream = new MemoryStream())
                {
                    browsing.SerializeJson(memStream, _jsonSerializer);
                    memStream.Seek(0, SeekOrigin.Begin);

                    result = memStream.ReadToString();
                }
            }

            return result;
        }

        // Support both JSON and XML for deserialization
        private static IList<IBrowseFilter> Deserialize(string value)
        {
            IList<IBrowseFilter> result = null;

            if (!string.IsNullOrEmpty(value))
            {
                FilteredBrowsing browsing;

                if (value.StartsWith("<"))
                {
                    // XML
                    var reader = new StringReader(value);
                    browsing = _xmlSerializer.Deserialize(reader) as FilteredBrowsing;
                }
                else
                {
                    // JSON
                    using (var stringReader = new StringReader(value))
                    using (var jsonTextReader = new JsonTextReader(stringReader))
                    {
                        browsing = _jsonSerializer.Deserialize<FilteredBrowsing>(jsonTextReader);
                    }
                }

                // Flatten groups
                if (browsing != null)
                {
                    result = new List<IBrowseFilter>();

                    if (browsing.Attributes != null)
                    {
                        result.AddRange(browsing.Attributes);
                    }

                    if (browsing.AttributeRanges != null)
                    {
                        result.AddRange(browsing.AttributeRanges);
                    }

                    if (browsing.Prices != null)
                    {
                        result.AddRange(browsing.Prices);
                    }
                }

            }

            return result?.OrderBy(f => f.Order).ToArray();
        }
    }
}

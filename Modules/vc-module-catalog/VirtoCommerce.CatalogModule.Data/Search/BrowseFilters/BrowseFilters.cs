using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace VirtoCommerce.CatalogModule.Data.Search.BrowseFilters
{
    [XmlRoot("browsing", Namespace = "", IsNullable = false)]
    public class FilteredBrowsing
    {
        [XmlElement("attribute")]
        public AttributeFilter[] Attributes { get; set; }

        [XmlElement("attributeRange")]
        public RangeFilter[] AttributeRanges { get; set; }

        [XmlElement("price")]
        public PriceRangeFilter[] Prices { get; set; }
    }

    public class AttributeFilter : IBrowseFilter
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlElement("facetSize")]
        public int? FacetSize { get; set; }

        public int Order { get; set; }

        [XmlElement("simple")]
        public AttributeFilterValue[] Values { get; set; }

        public IList<IBrowseFilterValue> GetValues()
        {
            return Values?.OfType<IBrowseFilterValue>().ToArray();
        }
    }

    [Serializable]
    public class AttributeFilterValue : IBrowseFilterValue
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }

    public class RangeFilter : IBrowseFilter
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        public int Order { get; set; }

        [XmlElement("range")]
        public RangeFilterValue[] Values { get; set; }

        public IList<IBrowseFilterValue> GetValues()
        {
            return Values?.OfType<IBrowseFilterValue>().ToArray();
        }
    }

    public class PriceRangeFilter : IBrowseFilter
    {
        [XmlIgnore]
        public string Key { get; } = "price";

        [XmlAttribute("currency")]
        public string Currency { get; set; }

        public int Order { get; set; }

        [XmlElement("range")]
        public RangeFilterValue[] Values { get; set; }

        public IList<IBrowseFilterValue> GetValues()
        {
            return Values?.OfType<IBrowseFilterValue>().ToArray();
        }
    }

    public class RangeFilterValue : IBrowseFilterValue
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("lower")]
        public string Lower { get; set; }

        [XmlAttribute("upper")]
        public string Upper { get; set; }

        [XmlAttribute("includeLower")]
        public bool IncludeLower { get; set; } = true; // The default value is 'true' for compatibility with previous ranges implementation

        [XmlAttribute("includeUpper")]
        public bool IncludeUpper { get; set; }
    }
}

using System;
using Microsoft.Azure.Search.Models;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public class AzureSearchRequest
    {
        public string AggregationId { get; set; }
        public string SearchText { get; set; }
        public SearchParameters SearchParameters { get; set; }
    }
}

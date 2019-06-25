using System;
using Microsoft.Azure.Search.Models;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public class AzureSearchResult
    {
        public string AggregationId { get; set; }
        public DocumentSearchResult ProviderResponse { get; set; }
    }
}

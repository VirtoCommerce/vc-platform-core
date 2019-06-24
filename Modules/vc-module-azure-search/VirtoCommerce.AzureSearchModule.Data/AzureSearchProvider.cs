using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest.Azure;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Exceptions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using DataType = Microsoft.Azure.Search.Models.DataType;
using IndexingResult = VirtoCommerce.SearchModule.Core.Model.IndexingResult;

namespace VirtoCommerce.AzureSearchModule.Data
{
    public class AzureSearchProvider : ISearchProvider
    {
        public const string ContentAnalyzerName = "content_analyzer";
        public const string NGramFilterName = "custom_ngram";
        public const string EdgeNGramFilterName = "custom_edge_ngram";

        private readonly AzureSearchOptions _azureSearchOptions;
        private readonly SearchOptions _searchOptions;
        private readonly ISettingsManager _settingsManager;
        private readonly Dictionary<string, IList<Field>> _mappings = new Dictionary<string, IList<Field>>();

        public AzureSearchProvider(IOptions<AzureSearchOptions> azureSearchOptions, IOptions<SearchOptions> searchOptions, ISettingsManager settingsManager)
        {
            if (azureSearchOptions == null)
                throw new ArgumentNullException(nameof(azureSearchOptions));

            if (searchOptions == null)
                throw new ArgumentNullException(nameof(searchOptions));

            _azureSearchOptions = azureSearchOptions.Value;
            _searchOptions = searchOptions.Value;

            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        }

        private SearchServiceClient _client;
        protected SearchServiceClient Client => _client ?? (_client = CreateSearchServiceClient());

        public virtual async Task DeleteIndexAsync(string documentType)
        {
            if (string.IsNullOrEmpty(documentType))
                throw new ArgumentNullException(nameof(documentType));

            try
            {
                var indexName = GetIndexName(documentType);

                if (await IndexExistsAsync(indexName))
                {
                    await Client.Indexes.DeleteAsync(indexName);
                }

                RemoveMappingFromCache(indexName);
            }
            catch (Exception ex)
            {
                ThrowException("Failed to delete index", ex);
            }
        }

        public virtual async Task<IndexingResult> IndexAsync(string documentType, IList<IndexDocument> documents)
        {
            var indexName = GetIndexName(documentType);

            var providerFields = await GetMappingAsync(indexName);
            var oldFieldsCount = providerFields.Count;

            var providerDocuments = documents.Select(document => ConvertToProviderDocument(document, providerFields, documentType)).ToList();

            var updateMapping = providerFields.Count != oldFieldsCount;
            var indexExits = await IndexExistsAsync(indexName);

            if (!indexExits)
            {
                await CreateIndex(indexName, providerFields);
            }

            if (updateMapping)
            {
                UpdateMapping(indexName, providerFields);
            }

            var result = await IndexWithRetryAsync(indexName, providerDocuments, 10);
            return result;
        }

        public virtual async Task<IndexingResult> RemoveAsync(string documentType, IList<IndexDocument> documents)
        {
            IndexingResult result;

            try
            {
                var indexName = GetIndexName(documentType);
                var indexClient = GetSearchIndexClient(indexName);
                var providerDocuments = documents.Select(document => ConvertToProviderDocument(document, null, documentType)).ToList();
                var batch = IndexBatch.Delete(providerDocuments);

                var response = await indexClient.Documents.IndexAsync(batch);
                result = CreateIndexingResult(response.Results);
            }
            catch (IndexBatchException ex)
            {
                result = CreateIndexingResult(ex.IndexingResults);
            }
            catch (Exception ex)
            {
                throw new SearchException(ex.Message, ex);
            }

            return result;
        }

        public virtual async Task<SearchResponse> SearchAsync(string documentType, SearchRequest request)
        {
            var indexName = GetIndexName(documentType);

            try
            {
                var availableFields = await GetMappingAsync(indexName);
                var indexClient = GetSearchIndexClient(indexName);

                var providerRequests = AzureSearchRequestBuilder.BuildRequest(request, indexName, documentType, availableFields);
                var providerResponses = await Task.WhenAll(providerRequests.Select(r => indexClient.Documents.SearchAsync(r?.SearchText, r?.SearchParameters)));

                // Copy aggregation ID from request to response
                var searchResults = providerResponses.Select((response, i) => new AzureSearchResult
                {
                    AggregationId = providerRequests[i].AggregationId,
                    ProviderResponse = response,
                })
                .ToArray();

                var result = searchResults.ToSearchResponse(request, documentType);
                return result;
            }
            catch (Exception ex)
            {
                throw new SearchException(ex.Message, ex);
            }
        }

        protected virtual SearchDocument ConvertToProviderDocument(IndexDocument document, IList<Field> providerFields, string documentType)
        {
            var result = new SearchDocument { Id = document.Id };

            document.Fields.Insert(0, new IndexDocumentField(AzureSearchHelper.RawKeyFieldName, document.Id) { IsRetrievable = true, IsFilterable = true });

            foreach (var field in document.Fields.OrderBy(f => f.Name))
            {
                var fieldName = AzureSearchHelper.ToAzureFieldName(field.Name);

                if (result.ContainsKey(fieldName))
                {
                    var newValues = new List<object>();

                    var currentValue = result[fieldName];

                    if (currentValue is object[] currentValues)
                    {
                        newValues.AddRange(currentValues);
                    }
                    else
                    {
                        newValues.Add(currentValue);
                    }

                    newValues.AddRange(field.Values);
                    result[fieldName] = newValues.ToArray();
                }
                else
                {
                    var providerField = AddProviderField(documentType, providerFields, fieldName, field);
                    var isCollection = providerField.Type.ToString().StartsWith("Collection(");

                    var point = field.Value as GeoPoint;
                    var value = point != null
                        ? (isCollection ? field.Values.Select(v => ((GeoPoint)v).ToDocumentValue()).ToArray() : point.ToDocumentValue())
                        : (isCollection ? field.Values : field.Value);

                    result.Add(fieldName, value);
                }
            }

            return result;
        }

        protected virtual Field AddProviderField(string documentType, IList<Field> providerFields, string fieldName, IndexDocumentField field)
        {
            var providerField = providerFields?.FirstOrDefault(f => f.Name == fieldName);

            if (providerField == null)
            {
                providerField = CreateProviderField(documentType, fieldName, field);
                providerFields?.Add(providerField);
            }

            return providerField;
        }

        protected virtual Field CreateProviderField(string documentType, string fieldName, IndexDocumentField field)
        {
            var originalFieldType = field.Value?.GetType() ?? typeof(object);
            var providerFieldType = GetProviderFieldType(documentType, fieldName, originalFieldType);

            var isGeoPoint = providerFieldType == DataType.GeographyPoint;
            var isString = providerFieldType == DataType.String;
            var isCollection = field.IsCollection && isString;

            if (isCollection)
            {
                providerFieldType = DataType.Collection(providerFieldType);
            }

            var providerField = new Field(fieldName, providerFieldType)
            {
                IsKey = fieldName == AzureSearchHelper.KeyFieldName,
                IsRetrievable = field.IsRetrievable,
                IsSearchable = isString && field.IsSearchable,
                IsFilterable = field.IsFilterable,
                IsFacetable = field.IsFilterable && !isGeoPoint,
                IsSortable = field.IsFilterable && !isCollection,
            };

            if (providerField.IsSearchable)
            {
                providerField.IndexAnalyzer = ContentAnalyzerName;
                providerField.SearchAnalyzer = AnalyzerName.StandardLucene;
            }

            return providerField;
        }

        protected virtual DataType GetProviderFieldType(string documentType, string fieldName, Type fieldType)
        {
            if (fieldType == typeof(string))
                return DataType.String;
            if (fieldType == typeof(int))
                return DataType.Int32;
            if (fieldType == typeof(long))
                return DataType.Int64;
            if (fieldType == typeof(double) || fieldType == typeof(decimal))
                return DataType.Double;
            if (fieldType == typeof(bool))
                return DataType.Boolean;
            if (fieldType == typeof(DateTimeOffset) || fieldType == typeof(DateTime))
                return DataType.DateTimeOffset;
            if (fieldType == typeof(GeoPoint))
                return DataType.GeographyPoint;

            throw new ArgumentException($"Field {fieldName} has unsupported type {fieldType}", nameof(fieldType));
        }

        protected virtual async Task<IndexingResult> IndexWithRetryAsync(string indexName, IEnumerable<SearchDocument> providerDocuments, int retryCount)
        {
            IndexingResult result = null;

            var batch = IndexBatch.Upload(providerDocuments);
            var indexClient = GetSearchIndexClient(indexName);

            // Retry if cannot index documents after updating the mapping
            for (var i = retryCount - 1; i >= 0; i--)
            {
                try
                {
                    var response = await indexClient.Documents.IndexAsync(batch);
                    result = CreateIndexingResult(response.Results);
                    break;
                }
                catch (IndexBatchException ex)
                {
                    result = CreateIndexingResult(ex.IndexingResults);
                    break;
                }
                catch (CloudException cloudException)
                    when (i > 0 && cloudException.Message.Contains("Make sure to only use property names that are defined by the type"))
                {
                    // Need to wait some time until new mapping is applied
                    await Task.Delay(1000);
                }
            }

            return result;
        }

        protected virtual IndexingResult CreateIndexingResult(IEnumerable<Microsoft.Azure.Search.Models.IndexingResult> results)
        {
            return new IndexingResult
            {
                Items = results.Select(r => new IndexingResultItem
                {
                    Id = r.Key,
                    Succeeded = r.Succeeded,
                    ErrorMessage = r.ErrorMessage,
                }).ToArray(),
            };
        }

        protected virtual string GetIndexName(string documentType)
        {
            // Use different index for each document type
            return string.Join("-", _searchOptions.Scope, documentType).ToLowerInvariant();
        }

        protected virtual Task<bool> IndexExistsAsync(string indexName)
        {
            return Client.Indexes.ExistsAsync(indexName);
        }

        #region Create and configure index

        protected virtual Task CreateIndex(string indexName, IList<Field> providerFields)
        {
            var index = CreateIndexDefinition(indexName, providerFields);
            return Client.Indexes.CreateAsync(index);
        }

        protected virtual Index CreateIndexDefinition(string indexName, IList<Field> providerFields)
        {
            var minGram = GetMinGram();
            var maxGram = GetMaxGram();

            var index = new Index
            {
                Name = indexName,
                Fields = providerFields.OrderBy(f => f.Name).ToArray(),
                TokenFilters = new TokenFilter[]
                {
                    new NGramTokenFilterV2(NGramFilterName, minGram, maxGram),
                    new EdgeNGramTokenFilterV2(EdgeNGramFilterName, minGram, maxGram)
                },
                Analyzers = new Analyzer[]
                {
                    new CustomAnalyzer
                    {
                        Name = ContentAnalyzerName,
                        Tokenizer = TokenizerName.Standard,
                        TokenFilters = new[] { TokenFilterName.Lowercase, GetTokenFilterName() }
                    }
                },
            };

            return index;
        }

        protected virtual string GetTokenFilterName()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.AzureSearch.TokenFilter", EdgeNGramFilterName);
        }

        protected virtual int GetMinGram()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.AzureSearch.NGramTokenFilter.MinGram", 1);
        }

        protected virtual int GetMaxGram()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.AzureSearch.NGramTokenFilter.MaxGram", 20);
        }

        #endregion

        #region Mapping

        protected virtual async Task<IList<Field>> GetMappingAsync(string indexName)
        {
            var providerFields = GetMappingFromCache(indexName);
            if (providerFields == null)
            {
                if (await IndexExistsAsync(indexName))
                {
                    var index = await Client.Indexes.GetAsync(indexName);
                    providerFields = index.Fields;
                }
            }

            providerFields = providerFields ?? new List<Field>();
            AddMappingToCache(indexName, providerFields);

            return providerFields;
        }

        protected virtual void UpdateMapping(string indexName, IList<Field> providerFields)
        {
            var index = CreateIndexDefinition(indexName, providerFields);
            var updatedIndex = Client.Indexes.CreateOrUpdate(indexName, index);

            AddMappingToCache(indexName, updatedIndex.Fields);
        }

        protected virtual IList<Field> GetMappingFromCache(string indexName)
        {
            return _mappings.ContainsKey(indexName) ? _mappings[indexName] : null;
        }

        protected virtual void AddMappingToCache(string indexName, IList<Field> providerFields)
        {
            _mappings[indexName] = providerFields;
        }

        protected virtual void RemoveMappingFromCache(string indexName)
        {
            _mappings.Remove(indexName);
        }

        #endregion

        protected virtual void ThrowException(string message, Exception innerException)
        {
            throw new SearchException($"{message}. Search service name: {_azureSearchOptions.SearchServiceName}, Scope: {_searchOptions.Scope}", innerException);
        }

        protected virtual SearchServiceClient CreateSearchServiceClient()
        {
            var result = new SearchServiceClient(_azureSearchOptions.SearchServiceName, new SearchCredentials(_azureSearchOptions.Key));
            return result;
        }

        protected virtual ISearchIndexClient GetSearchIndexClient(string indexName)
        {
            return Client.Indexes.GetClient(indexName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Exceptions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using SearchRequest = VirtoCommerce.SearchModule.Core.Model.SearchRequest;

namespace VirtoCommerce.ElasticSearchModule.Data
{
    public class ElasticSearchProvider : ISearchProvider
    {
        public const string SearchableFieldAnalyzerName = "searchable_field_analyzer";
        public const string NGramFilterName = "custom_ngram";
        public const string EdgeNGramFilterName = "custom_edge_ngram";

        private readonly ISettingsManager _settingsManager;
        private readonly Dictionary<string, Properties<IProperties>> _mappings = new Dictionary<string, Properties<IProperties>>();
        private readonly SearchOptions _searchOptions;
        private readonly ElasticSearchOptions _elasticSearchOptions;

        public ElasticSearchProvider(
            IOptions<ElasticSearchOptions> elasticSearchOptions,
            IOptions<SearchOptions> searchOptions,
            ISettingsManager settingsManager)
            : this(elasticSearchOptions, searchOptions, settingsManager, new ElasticClient(GetConnectionSettings(elasticSearchOptions.Value)))
        {
        }

        public ElasticSearchProvider(IOptions<ElasticSearchOptions> elasticSearchOptions,
            IOptions<SearchOptions> searchOptions, ISettingsManager settingsManager, IElasticClient client)
            : this(elasticSearchOptions, searchOptions, settingsManager, client, new ElasticSearchRequestBuilder())
        {
        }

        public ElasticSearchProvider(
            IOptions<ElasticSearchOptions> elasticSearchOptions, IOptions<SearchOptions> searchOptions,
            ISettingsManager settingsManager, IElasticClient client, ElasticSearchRequestBuilder requestBuilder)
        {
            if (searchOptions == null)
                throw new ArgumentNullException(nameof(searchOptions));

            if (elasticSearchOptions == null)
                throw new ArgumentNullException(nameof(elasticSearchOptions));

            _settingsManager = settingsManager;
            Client = client;
            RequestBuilder = requestBuilder;
            ServerUrl = client.ConnectionSettings.ConnectionPool.Nodes.First().Uri;
            _elasticSearchOptions = elasticSearchOptions.Value;
            _searchOptions = searchOptions.Value;
        }

        protected IElasticClient Client { get; }
        protected ElasticSearchRequestBuilder RequestBuilder { get; }
        protected Uri ServerUrl { get; }

        public virtual async Task DeleteIndexAsync(string documentType)
        {
            if (string.IsNullOrEmpty(documentType))
            {
                throw new ArgumentNullException(nameof(documentType));
            }

            try
            {
                var indexName = GetIndexName(documentType);

                var response = await Client.DeleteIndexAsync(indexName);
                if (!response.IsValid && response.ApiCall.HttpStatusCode != 404)
                {
                    throw new SearchException(response.DebugInformation);
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
            var providerFields = await GetMappingAsync(indexName, documentType);
            var oldFieldsCount = providerFields.Count();
            var providerDocuments = documents.Select(document => ConvertToProviderDocument(document, providerFields, documentType)).ToList();
            var updateMapping = providerFields.Count() != oldFieldsCount;
            var indexExits = await IndexExistsAsync(indexName);

            if (!indexExits)
            {
                await CreateIndexAsync(indexName, documentType);
            }

            if (!indexExits || updateMapping)
            {
                await UpdateMappingAsync(indexName, documentType, providerFields);
            }

            var bulkDefinition = new BulkDescriptor();
            bulkDefinition.IndexMany(providerDocuments).Index(indexName).Type(documentType);

            var bulkResponse = await Client.BulkAsync(bulkDefinition);
            await Client.RefreshAsync(indexName);

            var result = new IndexingResult
            {
                Items = bulkResponse.Items.Select(i => new IndexingResultItem
                {
                    Id = i.Id,
                    Succeeded = i.IsValid,
                    ErrorMessage = i.Error?.Reason
                }).ToArray()
            };

            return result;
        }

        public virtual async Task<IndexingResult> RemoveAsync(string documentType, IList<IndexDocument> documents)
        {
            var providerDocuments = documents.Select(d => new SearchDocument { Id = d.Id }).ToArray();
            var indexName = GetIndexName(documentType);
            var bulkDefinition = new BulkDescriptor();
            bulkDefinition.DeleteMany(providerDocuments).Index(indexName).Type(documentType);

            var bulkResponse = await Client.BulkAsync(bulkDefinition);
            await Client.RefreshAsync(indexName);

            var result = new IndexingResult
            {
                Items = bulkResponse.Items.Select(i => new IndexingResultItem
                {
                    Id = i.Id,
                    Succeeded = i.IsValid,
                    ErrorMessage = i.Error?.Reason
                }).ToArray()
            };

            return result;
        }

        public virtual async Task<SearchResponse> SearchAsync(string documentType, SearchRequest request)
        {
            var indexName = GetIndexName(documentType);
            ISearchResponse<SearchDocument> providerResponse;

            try
            {
                var availableFields = await GetMappingAsync(indexName, documentType);
                var providerRequest = RequestBuilder.BuildRequest(request, indexName, documentType, availableFields);
                providerResponse = await Client.SearchAsync<SearchDocument>(providerRequest);
            }
            catch (Exception ex)
            {
                throw new SearchException(ex.Message, ex);
            }

            if (!providerResponse.IsValid)
            {
                ThrowException(providerResponse.DebugInformation, null);
            }

            var result = providerResponse.ToSearchResponse(request, documentType);
            return result;
        }

        protected virtual SearchDocument ConvertToProviderDocument(IndexDocument document, Properties<IProperties> properties, string documentType)
        {
            var result = new SearchDocument { Id = document.Id };

            foreach (var field in document.Fields.OrderBy(f => f.Name))
            {
                var fieldName = ElasticSearchHelper.ToElasticFieldName(field.Name);

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
                    if (properties is IDictionary<PropertyName, IProperty> dictionary
                        && !dictionary.ContainsKey(fieldName))
                    {
                        // Create new property mapping
                        var providerField = CreateProviderField(field, documentType);
                        ConfigureProperty(providerField, field, documentType);
                        properties.Add(fieldName, providerField);
                    }

                    var isCollection = field.IsCollection || field.Values.Count > 1;

                    var point = field.Value as GeoPoint;
                    var value = point != null
                        ? (isCollection ? field.Values.Select(v => ((GeoPoint)v).ToElasticValue()).ToArray() : point.ToElasticValue())
                        : (isCollection ? field.Values : field.Value);

                    result.Add(fieldName, value);
                }
            }

            return result;
        }

        protected virtual IProperty CreateProviderField(IndexDocumentField field, string documentType)
        {
            var fieldType = field.Value?.GetType() ?? typeof(object);

            if (fieldType == typeof(string))
            {
                if (field.IsFilterable)
                    return new KeywordProperty();

                return new TextProperty();
            }

            switch (fieldType.Name)
            {
                case "Int32":
                case "UInt16":
                    return new NumberProperty(NumberType.Integer);
                case "Int16":
                case "Byte":
                    return new NumberProperty(NumberType.Short);
                case "SByte":
                    return new NumberProperty(NumberType.Byte);
                case "Int64":
                case "UInt32":
                case "TimeSpan":
                    return new NumberProperty(NumberType.Long);
                case "Single":
                    return new NumberProperty(NumberType.Float);
                case "Decimal":
                case "Double":
                case "UInt64":
                    return new NumberProperty(NumberType.Double);
                case "DateTime":
                case "DateTimeOffset":
                    return new DateProperty();
                case "Boolean":
                    return new BooleanProperty();
                case "Char":
                case "Guid":
                    return new KeywordProperty();
                case "GeoPoint":
                    return new GeoPointProperty();
            }

            throw new ArgumentException($"Field {field.Name} has unsupported type {fieldType}", nameof(field));
        }

        protected virtual void ConfigureProperty(IProperty property, IndexDocumentField field, string documentType)
        {
            if (property != null)
            {
                if (property is CorePropertyBase baseProperty)
                {
                    baseProperty.Store = field.IsRetrievable;
                }

                switch (property)
                {
                    case TextProperty textProperty:
                        ConfigureTextProperty(textProperty, field, documentType);
                        break;
                    case KeywordProperty keywordProperty:
                        ConfigureKeywordProperty(keywordProperty, field, documentType);
                        break;
                }
            }
        }

        protected virtual void ConfigureKeywordProperty(KeywordProperty keywordProperty, IndexDocumentField field, string documentType)
        {
            if (keywordProperty != null)
            {
                keywordProperty.Index = field.IsFilterable;
            }
        }

        protected virtual void ConfigureTextProperty(TextProperty textProperty, IndexDocumentField field, string documentType)
        {
            if (textProperty != null)
            {
                textProperty.Index = field.IsSearchable;
                textProperty.Analyzer = field.IsSearchable ? SearchableFieldAnalyzerName : null;
            }
        }

        protected virtual async Task<Properties<IProperties>> GetMappingAsync(string indexName, string documentType)
        {
            var properties = GetMappingFromCache(indexName);
            if (properties == null && await IndexExistsAsync(indexName))
            {
                var providerMapping = await Client.GetMappingAsync(new GetMappingRequest(indexName, documentType));
                var mapping = providerMapping.Mapping;
                if (mapping != null)
                {
                    properties = new Properties<IProperties>(mapping.Properties);
                }
            }

            properties = properties ?? new Properties<IProperties>();
            AddMappingToCache(indexName, properties);
            return properties;
        }

        protected virtual async Task UpdateMappingAsync(string indexName, string documentType, Properties<IProperties> properties)
        {
            var mappingRequest = new PutMappingRequest(indexName, documentType) { Properties = properties };
            var response = await Client.MapAsync(mappingRequest);

            if (!response.IsValid)
            {
                ThrowException("Failed to submit mapping. " + response.DebugInformation, response.OriginalException);
            }

            AddMappingToCache(indexName, properties);
            await Client.RefreshAsync(indexName);
        }

        protected virtual Properties<IProperties> GetMappingFromCache(string indexName)
        {
            return _mappings.ContainsKey(indexName) ? _mappings[indexName] : null;
        }

        protected virtual void AddMappingToCache(string indexName, Properties<IProperties> properties)
        {
            _mappings[indexName] = properties;
        }

        protected virtual void RemoveMappingFromCache(string indexName)
        {
            _mappings.Remove(indexName);
        }

        protected virtual string CreateCacheKey(params string[] parts)
        {
            return string.Join("/", parts);
        }

        protected virtual string[] ParseCacheKey(string key)
        {
            return key.Split('/');
        }

        protected virtual void ThrowException(string message, Exception innerException)
        {
            throw new SearchException($"{message}. URL:{ServerUrl}, Scope: {_searchOptions.Scope}", innerException);
        }

        protected virtual string GetIndexName(string documentType)
        {
            // Use different index for each document type
            return string.Join("-", _searchOptions.Scope, documentType).ToLowerInvariant();
        }

        protected virtual async Task<bool> IndexExistsAsync(string indexName)
        {
            var response = await Client.IndexExistsAsync(indexName);
            return response.Exists;
        }

        #region Create and configure index

        protected virtual async Task CreateIndexAsync(string indexName, string documentType)
        {
            await Client.CreateIndexAsync(indexName, i => i.Settings(s => ConfigureIndexSettings(s, documentType)));
        }

        protected virtual IndexSettingsDescriptor ConfigureIndexSettings(IndexSettingsDescriptor settings, string documentType)
        {
            // https://www.elastic.co/guide/en/elasticsearch/reference/current/mapping.html#mapping-limit-settings
            var fieldsLimit = GetFieldsLimit();

            return settings
                .Setting("index.mapping.total_fields.limit", fieldsLimit)
                .Analysis(a => a
                    .TokenFilters(tokenFilters => ConfigureTokenFilters(tokenFilters, documentType))
                    .Analyzers(analyzers => ConfigureAnalyzers(analyzers, documentType)));
        }

        protected virtual AnalyzersDescriptor ConfigureAnalyzers(AnalyzersDescriptor analyzers, string documentType)
        {
            return analyzers
                .Custom(SearchableFieldAnalyzerName, customAnalyzer => ConfigureSearchableFieldAnalyzer(customAnalyzer, documentType));
        }

        protected virtual CustomAnalyzerDescriptor ConfigureSearchableFieldAnalyzer(CustomAnalyzerDescriptor customAnalyzer, string documentType)
        {
            // Use ngrams analyzer for search in the middle of the word
            // http://www.elasticsearch.org/guide/en/elasticsearch/guide/current/ngrams-compound-words.html
            return customAnalyzer
                .Tokenizer("standard")
                .Filters("lowercase", GetTokenFilterName());
        }

        protected virtual TokenFiltersDescriptor ConfigureTokenFilters(TokenFiltersDescriptor tokenFilters, string documentType)
        {
            return tokenFilters
                .NGram(NGramFilterName, descriptor => ConfigureNGramFilter(descriptor, documentType))
                .EdgeNGram(EdgeNGramFilterName, descriptor => ConfigureEdgeNGramFilter(descriptor, documentType));
        }

        protected virtual NGramTokenFilterDescriptor ConfigureNGramFilter(NGramTokenFilterDescriptor nGram, string documentType)
        {
            return nGram.MinGram(GetMinGram()).MaxGram(GetMaxGram());
        }

        protected virtual EdgeNGramTokenFilterDescriptor ConfigureEdgeNGramFilter(EdgeNGramTokenFilterDescriptor edgeNGram, string documentType)
        {
            return edgeNGram.MinGram(GetMinGram()).MaxGram(GetMaxGram());
        }

        protected virtual int GetFieldsLimit()
        {
            var fieldsLimit = _settingsManager.GetValue("VirtoCommerce.Search.ElasticSearch.IndexTotalFieldsLimit", 1000);
            return fieldsLimit;
        }

        protected virtual string GetTokenFilterName()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.ElasticSearch.TokenFilter", EdgeNGramFilterName);
        }

        protected virtual int GetMinGram()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.ElasticSearch.NGramTokenFilter.MinGram", 1);
        }

        protected virtual int GetMaxGram()
        {
            return _settingsManager.GetValue("VirtoCommerce.Search.ElasticSearch.NGramTokenFilter.MaxGram", 20);
        }

        #endregion

        protected static IConnectionSettingsValues GetConnectionSettings(ElasticSearchOptions options)
        {
            var serverUrl = GetServerUrl(options);
            var accessUser = options.User;
            var accessKey = options.Key;
            var connectionSettings = new ConnectionSettings(serverUrl);

            if (!string.IsNullOrEmpty(accessUser) && !string.IsNullOrEmpty(accessKey))
            {
                connectionSettings.BasicAuthentication(accessUser, accessKey);
            }
            else if (!string.IsNullOrEmpty(accessKey))
            {
                // elastic is default name for elastic cloud
                connectionSettings.BasicAuthentication("elastic", accessKey);
            }

            if (options.EnableHttpCompression.EqualsInvariant("true"))
            {
                connectionSettings.EnableHttpCompression();
            }

            return connectionSettings;
        }

        protected static Uri GetServerUrl(ElasticSearchOptions options)
        {
            var server = options.Server;

            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException("'Server' parameter must not be empty");
            }

            if (!server.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !server.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                server = "http://" + server;
            }

            server = server.TrimEnd('/');
            return new Uri(server);
        }
    }
}

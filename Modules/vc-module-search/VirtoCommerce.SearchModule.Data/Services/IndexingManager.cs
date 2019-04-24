using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Data.Services
{
    /// <summary>
    /// Implement the functionality of indexing
    /// </summary>
    public class IndexingManager : IIndexingManager
    {
        private readonly ISearchProvider _searchProvider;
        private readonly IEnumerable<IndexDocumentConfiguration> _configs;
        private readonly ISettingsManager _settingsManager;
        private readonly IIndexingWorker _backgroundWorker;
        private readonly SearchOptions _searchOptions;

        public IndexingManager(ISearchProvider searchProvider, IEnumerable<IndexDocumentConfiguration> configs, IOptions<SearchOptions> searchOptions,
            ISettingsManager settingsManager = null, IIndexingWorker backgroundWorker = null)
        {
            if (searchProvider == null)
                throw new ArgumentNullException(nameof(searchProvider));
            if (configs == null)
                throw new ArgumentNullException(nameof(configs));

            _searchOptions = searchOptions.Value;
            _searchProvider = searchProvider;
            _configs = configs;
            _settingsManager = settingsManager;
            _backgroundWorker = backgroundWorker;
        }

        public virtual async Task<IndexState> GetIndexStateAsync(string documentType)
        {
            var result = new IndexState { DocumentType = documentType, Provider = _searchOptions.Provider, Scope = _searchOptions.Scope };

            var searchRequest = new SearchRequest
            {
                Sorting = new[] { new SortingField { FieldName = KnownDocumentFields.IndexationDate, IsDescending = true } },
                Take = 1,
            };

            try
            {
                var searchResponse = await _searchProvider.SearchAsync(documentType, searchRequest);

                result.IndexedDocumentsCount = searchResponse.TotalCount;

                if (searchResponse.Documents?.Any() == true)
                {
                    var indexationDate = searchResponse.Documents[0].FirstOrDefault(kvp => kvp.Key.EqualsInvariant(KnownDocumentFields.IndexationDate));
                    result.LastIndexationDate = Convert.ToDateTime(indexationDate.Value);
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        public virtual async Task IndexAsync(IndexingOptions options, Action<IndexingProgress> progressCallback, ICancellationToken cancellationToken)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.DocumentType))
                throw new ArgumentNullException($"{nameof(options)}.{nameof(options.DocumentType)}");

            if (options.BatchSize == null)
                options.BatchSize = _settingsManager?.GetValue(ModuleConstants.Settings.General.IndexPartitionSize.Name, 50) ?? 50;
            if (options.BatchSize < 1)
                throw new ArgumentException(@"Batch size cannon be less than 1", $"{nameof(options)}.{nameof(options.BatchSize)}");

            cancellationToken.ThrowIfCancellationRequested();

            var documentType = options.DocumentType;

            if (options.DeleteExistingIndex)
            {
                progressCallback?.Invoke(new IndexingProgress($"{documentType}: deleting index", documentType));
                await _searchProvider.DeleteIndexAsync(documentType);
            }

            var configs = _configs.Where(c => c.DocumentType.EqualsInvariant(documentType)).ToArray();

            foreach (var config in configs)
            {
                await ProcessConfigurationAsync(config, options, progressCallback, cancellationToken);
            }
        }

        public virtual async Task<IndexingResult> IndexDocumentsAsync(string documentType, string[] documentIds)
        {
            // Todo: reuse general index api?
            var configs = _configs.Where(c => c.DocumentType.EqualsInvariant(documentType)).ToArray();
            var result = new IndexingResult { Items = new List<IndexingResultItem>() };

            foreach (var config in configs)
            {
                var secondaryDocBuilders = config.RelatedSources?
                    .Where(s => s.DocumentBuilder != null)
                    .Select(s => s.DocumentBuilder)
                    .ToList();

                var configResult = await IndexDocumentsAsync(documentType, documentIds,
                    config.DocumentSource.DocumentBuilder, secondaryDocBuilders,
                    new CancellationTokenWrapper(CancellationToken.None));

                result.Items.AddRange(configResult.Items ?? Enumerable.Empty<IndexingResultItem>());
            }

            return result;
        }

        public virtual async Task<IndexingResult> DeleteDocumentsAsync(string documentType, string[] documentIds)
        {
            var documents = documentIds.Select(id => new IndexDocument(id)).ToList();
            return await _searchProvider.RemoveAsync(documentType, documents);
        }

        protected virtual async Task ProcessConfigurationAsync(IndexDocumentConfiguration configuration, IndexingOptions options, Action<IndexingProgress> progressCallback, ICancellationToken cancellationToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrEmpty(configuration.DocumentType))
                throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.DocumentType)}");
            if (configuration.DocumentSource == null)
                throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.DocumentSource)}");
            if (configuration.DocumentSource.ChangesProvider == null)
                throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.DocumentSource)}.{nameof(configuration.DocumentSource.ChangesProvider)}");
            if (configuration.DocumentSource.DocumentBuilder == null)
                throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.DocumentSource)}.{nameof(configuration.DocumentSource.DocumentBuilder)}");

            cancellationToken.ThrowIfCancellationRequested();

            var documentType = options.DocumentType;

            progressCallback?.Invoke(new IndexingProgress($"{documentType}: calculating total count", documentType));

            var batchOptions = new BatchIndexingOptions
            {
                DocumentType = options.DocumentType,
                PrimaryDocumentBuilder = configuration.DocumentSource.DocumentBuilder,
                SecondaryDocumentBuilders = configuration.RelatedSources
                    ?.Where(s => s.DocumentBuilder != null)
                    .Select(s => s.DocumentBuilder)
                    .ToList(),
            };

            var feeds = await GetChangeFeeds(configuration, options);

            // Try to get total count to indicate progress. Some feeds don't have a total count.
            var totalCount = feeds.Any(x => x.TotalCount == null)
                ? (long?)null
                : feeds.Sum(x => x.TotalCount ?? 0);

            long processedCount = 0;

            var changes = await GetNextChangesAsync(feeds);
            while (changes.Any())
            {
                IList<string> errors = null;

                if (_backgroundWorker == null)
                {
                    var indexingResult = await ProcessChangesAsync(changes, batchOptions, cancellationToken);
                    errors = GetIndexingErrors(indexingResult);
                }
                else
                {
                    // We're executing a job to index all documents or the changes since a specific time.
                    // Priority for this indexation work should be quite low.
                    var documentIds = changes
                        .Select(x => x.DocumentId)
                        .Distinct()
                        .ToArray();

                    _backgroundWorker.IndexDocuments(configuration.DocumentType, documentIds, IndexingPriority.Background);
                }

                processedCount += changes.Count;

                var description = totalCount != null
                    ? $"{documentType}: {processedCount} of {totalCount} have been indexed"
                    : $"{documentType}: {processedCount} have been indexed";

                progressCallback?.Invoke(new IndexingProgress(description, documentType, totalCount, processedCount, errors));

                changes = await GetNextChangesAsync(feeds);
            }

            progressCallback?.Invoke(new IndexingProgress($"{documentType}: indexation finished", documentType, totalCount ?? processedCount, processedCount));
        }

        protected virtual async Task<IList<IndexDocumentChange>> GetNextChangesAsync(IList<IIndexDocumentChangeFeed> feeds)
        {
            var batches = await Task.WhenAll(feeds.Select(f => f.GetNextBatch()));

            var changes = batches
                .Where(b => b != null)
                .SelectMany(b => b)
                .ToList();

            return changes;
        }

        protected virtual async Task<IndexingResult> ProcessChangesAsync(IEnumerable<IndexDocumentChange> changes, BatchIndexingOptions batchOptions, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new IndexingResult();

            var groups = GetLatestChangesForEachDocumentGroupedByChangeType(changes);

            foreach (var group in groups)
            {
                var changeType = group.Key;
                var documentIds = group.Value;

                var groupResult = await ProcessDocumentsAsync(changeType, documentIds, batchOptions, cancellationToken);

                if (groupResult?.Items != null)
                {
                    if (result.Items == null)
                    {
                        result.Items = new List<IndexingResultItem>();
                    }

                    result.Items.AddRange(groupResult.Items);
                }
            }

            return result;
        }

        protected virtual async Task<IndexingResult> ProcessDocumentsAsync(IndexDocumentChangeType changeType, IList<string> documentIds, BatchIndexingOptions batchOptions, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IndexingResult result = null;

            if (changeType == IndexDocumentChangeType.Deleted)
            {
                result = await DeleteDocumentsAsync(batchOptions.DocumentType, documentIds.ToArray());
            }
            else if (changeType == IndexDocumentChangeType.Modified)
            {
                result = await IndexDocumentsAsync(batchOptions.DocumentType, documentIds, batchOptions.PrimaryDocumentBuilder, batchOptions.SecondaryDocumentBuilders, cancellationToken);
            }

            return result;
        }

        protected virtual async Task<IndexingResult> IndexDocumentsAsync(string documentType, IList<string> documentIds, IIndexDocumentBuilder primaryDocumentBuilder, IEnumerable<IIndexDocumentBuilder> secondaryDocumentBuilders, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var documents = await GetDocumentsAsync(documentIds, primaryDocumentBuilder, secondaryDocumentBuilders, cancellationToken);
            var response = await _searchProvider.IndexAsync(documentType, documents);
            return response;
        }

        protected virtual async Task<IIndexDocumentChangeFeed[]> GetChangeFeeds(IndexDocumentConfiguration configuration, IndexingOptions options)
        {
            // Return in-memory change feed for specific set of document ids.
            if (options.DocumentIds != null)
            {
                return new IIndexDocumentChangeFeed[]
                {
                    new InMemoryIndexDocumentChangeFeed(options.DocumentIds.ToArray(), IndexDocumentChangeType.Modified, options.BatchSize ?? 50)
                };
            }

            // Support old ChangesProvider.
            if (configuration.DocumentSource.ChangeFeedFactory == null)
            {
                configuration.DocumentSource.ChangeFeedFactory = new IndexDocumentChangeFeedFactoryAdapter(configuration.DocumentSource.ChangesProvider);
            }

            var factories = new List<IIndexDocumentChangeFeedFactory>
            {
                configuration.DocumentSource.ChangeFeedFactory
            };

            // In case of 'full' re-index we don't want to include the related sources,
            // because that would double the indexation work.
            // E.g. All products would get indexed for the primary document source
            // and afterwards all products would get re-indexed for all the prices as well.
            if (options.StartDate != null || options.EndDate != null)
            {
                foreach (var related in configuration.RelatedSources ?? Enumerable.Empty<IndexDocumentSource>())
                {
                    // Support old ChangesProvider.
                    if (related.ChangeFeedFactory == null)
                        related.ChangeFeedFactory = new IndexDocumentChangeFeedFactoryAdapter(related.ChangesProvider);

                    factories.Add(related.ChangeFeedFactory);
                }
            }

            return await Task.WhenAll(factories.Select(x => x.CreateFeed(options.StartDate, options.EndDate, options.BatchSize ?? 50)));
        }

        protected virtual IList<string> GetIndexingErrors(IndexingResult indexingResult)
        {
            var errors = indexingResult?.Items?
                .Where(i => !i.Succeeded)
                .Select(i => $"ID: {i.Id}, Error: {i.ErrorMessage}")
                .ToList();

            return errors?.Any() == true ? errors : null;
        }

        protected virtual IDictionary<IndexDocumentChangeType, string[]> GetLatestChangesForEachDocumentGroupedByChangeType(IEnumerable<IndexDocumentChange> changes)
        {
            var result = changes
                .GroupBy(c => c.DocumentId)
                .Select(g => g.OrderByDescending(o => o.ChangeDate).First())
                .GroupBy(c => c.ChangeType)
                .ToDictionary(g => g.Key, g => g.Select(c => c.DocumentId).ToArray());

            return result;
        }

        protected virtual async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds, IIndexDocumentBuilder primaryDocumentBuilder, IEnumerable<IIndexDocumentBuilder> secondaryDocumentBuilders, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var primaryDocuments = (await primaryDocumentBuilder.GetDocumentsAsync(documentIds))
                ?.Where(d => d != null)
                .ToList();

            if (primaryDocuments?.Any() == true)
            {
                if (secondaryDocumentBuilders != null)
                {
                    var primaryDocumentIds = primaryDocuments.Select(d => d.Id).ToArray();
                    var secondaryDocuments = await GetSecondaryDocumentsAsync(secondaryDocumentBuilders, primaryDocumentIds, cancellationToken);

                    MergeDocuments(primaryDocuments, secondaryDocuments);
                }

                // Add system fields
                foreach (var document in primaryDocuments)
                {
                    document.Add(new IndexDocumentField(KnownDocumentFields.IndexationDate, DateTime.UtcNow)
                    {
                        IsRetrievable = true,
                        IsFilterable = true
                    });
                }
            }

            return primaryDocuments;
        }

        protected virtual async Task<IList<IndexDocument>> GetSecondaryDocumentsAsync(IEnumerable<IIndexDocumentBuilder> secondaryDocumentBuilders, IList<string> documentIds, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tasks = secondaryDocumentBuilders.Select(p => p.GetDocumentsAsync(documentIds));
            var results = await Task.WhenAll(tasks);

            var result = results
                .Where(r => r != null)
                .SelectMany(r => r.Where(d => d != null))
                .ToList();

            return result;
        }

        protected virtual void MergeDocuments(IList<IndexDocument> primaryDocuments, IList<IndexDocument> secondaryDocuments)
        {
            if (primaryDocuments?.Any() == true && secondaryDocuments?.Any() == true)
            {
                var secondaryDocumentGroups = secondaryDocuments
                    .GroupBy(d => d.Id)
                    .ToDictionary(g => g.Key, g => g, StringComparer.OrdinalIgnoreCase);

                foreach (var primaryDocument in primaryDocuments)
                {
                    if (secondaryDocumentGroups.ContainsKey(primaryDocument.Id))
                    {
                        var secondaryDocumentGroup = secondaryDocumentGroups[primaryDocument.Id];

                        foreach (var secondaryDocument in secondaryDocumentGroup)
                        {
                            primaryDocument.Merge(secondaryDocument);
                        }
                    }
                }
            }
        }
    }
}

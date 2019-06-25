using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Services;
using Xunit;

namespace VirtoCommerce.SearchModule.Tests
{
    [Trait("Category", "Unit")]
    public class IndexingManagerTests
    {
        public const string Rebuild = "rebuild";
        public const string Update = "update";
        public const string Primary = "primary";
        public const string Secondary = "secondary";
        public const string DocumentType = "item";

        [Theory]
        [InlineData(Rebuild, 1, Primary)]
        [InlineData(Rebuild, 3, Primary)]
        [InlineData(Update, 1, Primary)]
        [InlineData(Update, 3, Primary)]
        [InlineData(Rebuild, 1, Primary, Secondary)]
        [InlineData(Rebuild, 3, Primary, Secondary)]
        [InlineData(Update, 1, Primary, Secondary)]
        [InlineData(Update, 3, Primary, Secondary)]
        public async Task CanIndexAllDocuments(string operation, int batchSize, params string[] sourceNames)
        {
            var rebuild = operation == Rebuild;

            var searchProvider = new SearchProvider();
            var documentSources = GetDocumentSources(sourceNames);
            var manager = GetIndexingManager(searchProvider, documentSources);
            var progress = new List<IndexingProgress>();
            var cancellationTokenSource = new CancellationTokenSource();

            var options = new IndexingOptions
            {
                DocumentType = DocumentType,
                DeleteExistingIndex = rebuild,
                StartDate = rebuild ? null : (DateTime?)new DateTime(1, 1, 1),
                EndDate = rebuild ? null : (DateTime?)new DateTime(1, 1, 9),
                BatchSize = batchSize,
            };

            await manager.IndexAsync(options, p => progress.Add(p), new CancellationTokenWrapper(cancellationTokenSource.Token));

            var expectedBatchesCount = GetExpectedBatchesCount(rebuild, documentSources, batchSize);
            var expectedProgressItemsCount = (rebuild ? 1 : 0) + 1 + expectedBatchesCount + 1;

            Assert.Equal(expectedProgressItemsCount, progress.Count);

            var i = 0;

            if (rebuild)
            {
                Assert.Equal($"{DocumentType}: deleting index", progress[i++].Description);
            }

            Assert.Equal($"{DocumentType}: calculating total count", progress[i++].Description);

            for (var batch = 0; batch < expectedBatchesCount; batch++)
            {
                var progressItem = progress[i++];
                Assert.Equal($"{DocumentType}: {progressItem.ProcessedCount} of {progressItem.TotalCount} have been indexed", progressItem.Description);
            }

            Assert.Equal($"{DocumentType}: indexation finished", progress[i].Description);

            ValidateErrors(progress, "bad1");

            var expectedFieldNames = new List<string>(sourceNames) { KnownDocumentFields.IndexationDate };
            ValidateIndexedDocuments(searchProvider.IndexedDocuments.Values, expectedFieldNames, "good2", "good3");
        }

        [Theory]
        [InlineData(1, Primary)]
        [InlineData(3, Primary)]
        [InlineData(1, Primary, Secondary)]
        [InlineData(3, Primary, Secondary)]
        public async Task CanIndexSpecificDocuments(int batchSize, params string[] sourceNames)
        {
            var searchProvider = new SearchProvider();
            var documentSources = GetDocumentSources(sourceNames);
            var manager = GetIndexingManager(searchProvider, documentSources);
            var progress = new List<IndexingProgress>();
            var cancellationTokenSource = new CancellationTokenSource();

            var options = new IndexingOptions
            {
                DocumentType = DocumentType,
                DocumentIds = new[] { "bad1", "good3", "non-existent-id" },
                BatchSize = batchSize,
            };

            await manager.IndexAsync(options, p => progress.Add(p), new CancellationTokenWrapper(cancellationTokenSource.Token));

            var expectedBatchesCount = GetBatchesCount(options.DocumentIds.Count, batchSize);
            var expectedProgressItemsCount = 1 + expectedBatchesCount + 1;

            Assert.Equal(expectedProgressItemsCount, progress.Count);

            var i = 0;

            Assert.Equal($"{DocumentType}: calculating total count", progress[i++].Description);

            for (var batch = 0; batch < expectedBatchesCount; batch++)
            {
                var progressItem = progress[i++];
                Assert.Equal($"{DocumentType}: {progressItem.ProcessedCount} of {progressItem.TotalCount} have been indexed", progressItem.Description);
            }

            Assert.Equal($"{DocumentType}: indexation finished", progress[i].Description);

            ValidateErrors(progress, "bad1");

            var expectedFieldNames = new List<string>(sourceNames) { KnownDocumentFields.IndexationDate };
            ValidateIndexedDocuments(searchProvider.IndexedDocuments.Values, expectedFieldNames, "good3");
        }


        private static IList<DocumentSource> GetDocumentSources(IEnumerable<string> names)
        {
            return names.Select(GetDocumentSource).ToArray();
        }

        private static DocumentSource GetDocumentSource(string name)
        {
            switch (name)
            {
                case Primary:
                    return new DocumentSource(name)
                    {
                        DocumentIds = new[]
                        {
                            "bad1",
                            "good2",
                            "good3",
                        },
                        Changes = new[]
                        {
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 1), DocumentId = "bad1", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 2), DocumentId = "good1", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 3), DocumentId = "good1", ChangeType = IndexDocumentChangeType.Deleted },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 4), DocumentId = "good2", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 5), DocumentId = "good3", ChangeType = IndexDocumentChangeType.Modified },
                        }
                    };
                case Secondary:
                    return new DocumentSource(name)
                    {
                        Changes = new[]
                        {
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 2), DocumentId = "bad1", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 3), DocumentId = "good1", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 4), DocumentId = "good1", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 5), DocumentId = "good2", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 6), DocumentId = "good2", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 7), DocumentId = "good3", ChangeType = IndexDocumentChangeType.Modified },
                            new IndexDocumentChange { ChangeDate = new DateTime(1, 1, 8), DocumentId = "good3", ChangeType = IndexDocumentChangeType.Modified },
                        }
                    };
            }

            return null;
        }

        private static int GetExpectedBatchesCount(bool rebuild, IEnumerable<DocumentSource> documentSources, int batchSize)
        {
            int result;

            if (rebuild)
            {
                // Use documents count from primary source
                result = GetBatchesCount(documentSources?.FirstOrDefault()?.DocumentIds.Count ?? 0, batchSize);
            }
            else
            {
                // Calculate batches count for each source and return the maximum value
                result = documentSources?.Max(s => GetBatchesCount(s?.Changes.Count ?? 0, batchSize)) ?? 0;
            }

            return result;
        }

        private static int GetBatchesCount(int itemsCount, int batchSize)
        {
            return (int)Math.Ceiling((decimal)itemsCount / batchSize);
        }

        private static void ValidateErrors(IEnumerable<IndexingProgress> progress, params string[] expectdErrorDoucmentIds)
        {
            var errors = progress
                .Where(p => p.Errors != null)
                .SelectMany(p => p.Errors)
                .ToList();

            Assert.Equal(expectdErrorDoucmentIds.Length, errors.Count);

            foreach (var doucmentId in expectdErrorDoucmentIds)
            {
                Assert.Equal($"ID: {doucmentId}, Error: Search provider error", errors[0]);
            }
        }


        private static void ValidateIndexedDocuments(ICollection<IndexDocument> documents, ICollection<string> expectedFieldNames, params string[] expectedDocumentIds)
        {
            Assert.Equal(expectedDocumentIds.Length, documents.Count);

            foreach (var document in documents)
            {
                Assert.NotNull(document);
                Assert.Contains(document.Id, expectedDocumentIds);
                Assert.NotNull(document.Fields);
                Assert.Equal(expectedFieldNames.Count, document.Fields.Count);

                foreach (var fieldName in expectedFieldNames)
                {
                    var field = document.Fields.FirstOrDefault(f => f.Name == fieldName);

                    Assert.NotNull(field);

                    if (!fieldName.EqualsInvariant(KnownDocumentFields.IndexationDate))
                    {
                        Assert.Equal(document.Id, field.Value);
                    }
                }
            }
        }


        private static IIndexingManager GetIndexingManager(ISearchProvider searchProvider, IList<DocumentSource> documentSources)
        {
            var primaryDocumentSource = documentSources?.FirstOrDefault();

            var configuration = new IndexDocumentConfiguration
            {
                DocumentType = DocumentType,
                DocumentSource = CreateIndexDocumentSource(primaryDocumentSource),
                RelatedSources = documentSources?.Skip(1).Select(CreateIndexDocumentSource).ToArray(),
            };

            return new IndexingManager(searchProvider, new[] { configuration }, new Moq.Mock<IOptions<SearchOptions>>().Object);
        }

        private static IndexDocumentSource CreateIndexDocumentSource(DocumentSource documentSource)
        {
            return new IndexDocumentSource
            {
                ChangesProvider = documentSource,
                DocumentBuilder = documentSource,
            };
        }
    }
}

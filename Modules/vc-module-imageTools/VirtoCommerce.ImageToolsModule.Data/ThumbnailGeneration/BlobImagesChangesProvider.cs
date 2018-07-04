using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    public class BlobImagesChangesProvider : IImagesChangesProvider
    {
        public bool IsTotalCountSupported => true;

        private readonly IBlobStorageProvider _storageProvider;
        private readonly IThumbnailOptionSearchService _thumbnailOptionSearchService;

        private IList<ImageChange> _changeBlobs;

        public BlobImagesChangesProvider(IBlobStorageProvider storageProvider, IThumbnailOptionSearchService thumbnailOptionSearchService)
        {
            _storageProvider = storageProvider;
            _thumbnailOptionSearchService = thumbnailOptionSearchService;
        }

        protected virtual async Task<IList<ImageChange>> GetChangeFiles(ThumbnailTask task, DateTime? changedSince,
            ICancellationToken token)
        {
            var options = await GetOptionsCollection();
            var allBlobInfos = await ReadBlobFolderAsync(task.WorkPath, token);
            var orignalBlobInfos = GetOriginalItems(allBlobInfos, options.Select(x => x.FileSuffix).ToList());

            var result = new List<ImageChange>();
            foreach (var blobInfo in orignalBlobInfos)
            {
                token?.ThrowIfCancellationRequested();

                var imageChange = new ImageChange
                {
                    Name = blobInfo.Name,
                    Url = blobInfo.Url,
                    ModifiedDate = blobInfo.ModifiedDate,
                    ChangeState = !changedSince.HasValue ? EntryState.Added : GetItemState(blobInfo, changedSince, task.ThumbnailOptions)
                };
                result.Add(imageChange);
            }
            return result.Where(x => x.ChangeState != EntryState.Unchanged).ToList();
        }

        #region Implementation of IImagesChangesProvider

        public async Task<long> GetTotalChangesCount(ThumbnailTask task, DateTime? changedSince,
            ICancellationToken token)
        {
            if (_changeBlobs == null)
            {
                _changeBlobs = await GetChangeFiles(task, changedSince, token);
            }
            return _changeBlobs.Count;
        }

        public async Task<ImageChange[]> GetNextChangesBatch(ThumbnailTask task, DateTime? changedSince, long? skip,
            long? take, ICancellationToken token)
        {
            if (_changeBlobs == null)
            {
                _changeBlobs = await GetChangeFiles(task, changedSince, token);
            }

            var count = _changeBlobs.Count;

            if (skip >= count)
            {
                return new ImageChange[] { };
            }

            return _changeBlobs.Skip((int)skip).Take((int)take).ToArray();
        }

        #endregion

        protected virtual async Task<ICollection<BlobEntry>> ReadBlobFolderAsync(string folderPath, ICancellationToken token)
        {
            token?.ThrowIfCancellationRequested();

            var result = new List<BlobEntry>();

            var searchResults = await _storageProvider.SearchAsync(folderPath, null);

            result.AddRange(searchResults.Results);
            foreach (var blobFolder in searchResults.Results.Where( x => x.Type == "folder"))
            {
                var folderResult = await ReadBlobFolderAsync(blobFolder.RelativeUrl, token);
                result.AddRange(folderResult);
            }

            return result;
        }

        /// <summary>
        /// Check if image is exist in blob storage by url.
        /// </summary>
        /// <param name="imageUrl">Image url.</param>
        /// <returns>
        /// EntryState if image exist.
        /// Null is image is empty
        /// </returns>
        protected virtual async Task<bool> ExistsAsync(string imageUrl)
        {
            var blobInfo = await _storageProvider.GetBlobInfoAsync(imageUrl);
            return blobInfo != null;
        }

        protected virtual EntryState GetItemState(BlobEntry blobInfo, DateTime? changedSince, IList<ThumbnailOption> options)
        {
            if (!changedSince.HasValue)
            {
                return EntryState.Added;
            }

            foreach (var option in options)
            {
                if (!ExistsAsync(blobInfo.Url.GenerateThumbnailName(option.FileSuffix)).GetAwaiter().GetResult())
                {
                    return EntryState.Added;
                }
            }

            if (blobInfo.ModifiedDate.HasValue && blobInfo.ModifiedDate >= changedSince)
            {
                return EntryState.Modified;
            }

            return EntryState.Unchanged;
        }

        //get all options to create a map of all potential file names
        protected virtual async Task<ICollection<ThumbnailOption>> GetOptionsCollection()
        {
            var options = await _thumbnailOptionSearchService.SearchAsync(new ThumbnailOptionSearchCriteria()
            {
                Take = Int32.MaxValue
            });

            return options.Results.ToList();
        }

        /// <summary>
        /// Calculate the original images
        /// </summary>
        /// <param name="source"></param>
        /// <param name="suffixCollection"></param>
        /// <returns></returns>
        protected virtual ICollection<BlobEntry> GetOriginalItems(ICollection<BlobEntry> source, ICollection<string> suffixCollection)
        {
            var result = new List<BlobEntry>();

            foreach (var blobInfo in source)
            {
                var present = false;
                foreach (var suffix in suffixCollection)
                {
                    var name = blobInfo.Name;
                    if (name.Contains("_" + suffix))
                    {
                        present = true;
                        break;
                    }
                }

                if (!present)
                {
                    result.Add(blobInfo);
                }
            }

            return result;
        }
    }
}

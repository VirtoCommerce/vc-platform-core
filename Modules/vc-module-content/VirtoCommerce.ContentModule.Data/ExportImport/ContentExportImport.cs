using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ContentModule.Data.ExportImport
{
    public sealed class ContentExportImport
    {
        private static string[] _exportedFolders = { "Pages", "Themes" };
        private readonly IMenuService _menuService;
        private readonly IContentStorageProviderFactory _contentStorageProvider;
        private readonly JsonSerializer _jsonSerializer;
        private readonly int _batchSize = 50;

        public ContentExportImport(IMenuService menuService, Func<string, IContentStorageProviderFactory> contentStorageProviderFactory, JsonSerializer jsonSerializer)
        {
            if (contentStorageProviderFactory == null)
                throw new ArgumentNullException(nameof(contentStorageProviderFactory));

            _menuService = menuService;
            _jsonSerializer = jsonSerializer;
            _contentStorageProvider = contentStorageProviderFactory(string.Empty);
        }

        public async Task DoExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                await writer.WriteStartObjectAsync();

                //Export menu link list
                var menuLinkLists = await _menuService.GetAllLinkListsAsync();
                var linkLists = menuLinkLists as IList<MenuLinkList> ?? menuLinkLists.ToList();

                await writer.WritePropertyNameAsync("MenuLinkLists");
                await writer.WriteStartArrayAsync();

                for (var skip = 0; skip < linkLists.Count; skip += _batchSize)
                {
                    progressInfo.Description = $"{skip} of {linkLists.Count} menu link lists have been loaded";
                    progressCallback(progressInfo);

                    foreach (var list in linkLists.Skip(skip).Take(_batchSize).ToList())
                    {
                        _jsonSerializer.Serialize(writer, list);
                    }

                    await writer.FlushAsync();
                    progressInfo.Description = $"{ Math.Min(linkLists.Count, skip + _batchSize) } of { linkLists.Count } menu link lists exported";
                    progressCallback(progressInfo);
                }

                await writer.WriteEndArrayAsync();

                if (options.HandleBinaryData)
                {
                    await writer.WritePropertyNameAsync("CmsContent");
                    await writer.WriteStartArrayAsync();

                    var backupContentFolders = new List<ContentFolder>();
                    var result = await _contentStorageProvider.SearchAsync(string.Empty, null);
                    foreach (var blobFolder in result.Results.Where(x => _exportedFolders.Contains(x.Name)))
                    {
                        var contentFolder = new ContentFolder
                        {
                            Url = blobFolder.RelativeUrl
                        };
                        ReadContentFoldersRecurive(contentFolder, progressCallback);
                        backupContentFolders.Add(contentFolder);
                    }

                    _jsonSerializer.Serialize(writer, backupContentFolders);
                    await writer.FlushAsync();

                    progressInfo.Description = $"{ result.TotalCount } cms content exported";
                    progressCallback(progressInfo);

                    await writer.WriteEndArrayAsync();
                }

                await writer.WriteEndObjectAsync();
                await writer.FlushAsync();
            }
        }

        //TODO
        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        //public void DoImport(Stream backupStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        //{
        //    if (manifest == null)
        //        throw new ArgumentNullException(nameof(manifest));

        //    var backupObject = backupStream.DeserializeJson<BackupObject>();
        //    var originalObject = GetBackupObject(progressCallback, false);

        //    var progressInfo = new ExportImportProgressInfo
        //    {
        //        Description = String.Format("{0} menu link lists importing...", backupObject.MenuLinkLists.Count())
        //    };
        //    progressCallback(progressInfo);
        //    UpdateMenuLinkLists(backupObject.MenuLinkLists);

        //    if (manifest.HandleBinaryData)
        //    {
        //        progressInfo.Description = String.Format("importing binary data:  themes and pages importing...");
        //        progressCallback(progressInfo);
        //        foreach (var folder in backupObject.ContentFolders)
        //        {
        //            SaveContentFolderRecursive(folder, progressCallback);
        //        }
        //    }
        //}

        //private void UpdateMenuLinkLists(ICollection<webModels.MenuLinkList> linkLIsts)
        //{
        //    foreach (var item in linkLIsts.Select(x => x.ToCoreModel()))
        //    {
        //        _menuService.AddOrUpdate(item);
        //    }
        //}

        private async Task<BackupObject> GetBackupObjectAsync(Action<ExportImportProgressInfo> progressCallback, bool handleBynaryData)
        {
            var retVal = new BackupObject();

            var progressInfo = new ExportImportProgressInfo
            {
                Description = "cms content loading..."
            };
            progressCallback(progressInfo);

            var menuLinkLists = await _menuService.GetAllLinkListsAsync();
            retVal.MenuLinkLists = menuLinkLists.ToList();

            //if (handleBynaryData)
            //{
            //    var result = await _contentStorageProviderFactory.SearchAsync("", null);
            //    foreach (var blobFolder in result.Results.OfType<BlobFolder>().Where(x => _exportedFolders.Contains(x.Name)))
            //    {
            //        var contentFolder = new ContentFolder
            //        {
            //            Url = blobFolder.RelativeUrl
            //        };
            //        ReadContentFoldersRecurive(contentFolder, progressCallback);
            //        retVal.ContentFolders.Add(contentFolder);
            //    }
            //}

            return retVal;
        }

        //private void SaveContentFolderRecursive(ContentFolder folder, Action<ExportImportProgressInfo> progressCallback)
        //{
        //    foreach (var childFolder in folder.Folders)
        //    {
        //        SaveContentFolderRecursive(childFolder, progressCallback);
        //    }
        //    foreach (var folderFile in folder.Files)
        //    {
        //        using (var stream = _contentStorageProvider.OpenWrite(folderFile.Url))
        //        using (var memStream = new MemoryStream(folderFile.Data))
        //        {
        //            var progressInfo = new ExportImportProgressInfo
        //            {
        //                Description = String.Format("Saving {0}", folderFile.Url)
        //            };
        //            progressCallback(progressInfo);
        //            memStream.CopyTo(stream);
        //        }
        //    }
        //}

        private void ReadContentFoldersRecurive(ContentFolder folder, Action<ExportImportProgressInfo> progressCallback)
        {
            var result = _contentStorageProvider.SearchAsync(folder.Url, null).GetAwaiter().GetResult();

            foreach (var blobFolder in result.Results.OfType<BlobFolder>())
            {
                var contentFolder = new ContentFolder()
                {
                    Url = blobFolder.RelativeUrl
                };

                ReadContentFoldersRecurive(contentFolder, progressCallback);
                folder.Folders.Add(contentFolder);
            }

            foreach (var blobItem in result.Results)
            {
                var progressInfo = new ExportImportProgressInfo
                {
                    Description = $"Read {blobItem.Url}"
                };
                progressCallback(progressInfo);

                var contentFile = new ContentFile
                {
                    Url = blobItem.RelativeUrl
                };
                using (var stream = _contentStorageProvider.OpenRead(blobItem.Url))
                {
                    contentFile.Data = stream.ReadFully();
                }
                folder.Files.Add(contentFile);
            }
        }
    }
}

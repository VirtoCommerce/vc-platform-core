using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ContentModule.Data.ExportImport
{
    public sealed class ContentExportImport : IExportSupport, IImportSupport
    {
        private static string[] _exportedFolders = { "Pages", "Themes" };
        private readonly IMenuService _menuService;
        private readonly IContentStorageProviderFactory _contentStorageProviderFactory;
        private readonly JsonSerializer _serializer;
        private readonly int BatchSize = 50;

        public ContentExportImport(IMenuService menuService, Func<string, IContentStorageProviderFactory> themesStorageProviderFactory, JsonSerializer jsonSerializer)
        {
            if (themesStorageProviderFactory == null)
                throw new ArgumentNullException(nameof(themesStorageProviderFactory));

            _menuService = menuService;
            _serializer = jsonSerializer;
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();

            //ToDo make export import
            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                var backupObject = await GetBackupObjectAsync(progressCallback, options.HandleBinaryData);

                writer.WriteStartObject();

                //Export menu link list
                var countLinkList = backupObject.MenuLinkLists.Count;

                writer.WriteStartArray();
                writer.WritePropertyName("MenuLinkLists");

                for (int i = BatchSize; i < countLinkList; i += BatchSize)
                {
                    progressInfo.Description = $"{i} of {countLinkList} menu link lists have been loaded";
                    progressCallback(progressInfo);

                    foreach (var list in backupObject.MenuLinkLists)
                    {
                        _serializer.Serialize(writer, list);
                    }

                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(countLinkList, i + BatchSize) } of { countLinkList } menu link lists exported";
                    progressCallback(progressInfo);

                }
                writer.WriteEndArray();



                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //public void DoExport(Stream backupStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        //{
        //    if (manifest == null)
        //        throw new ArgumentNullException(nameof(manifest));

        //    var backupObject = GetBackupObject(progressCallback, manifest.HandleBinaryData);
        //    backupObject.SerializeJson(backupStream);
        //}

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
            var result = _contentStorageProviderFactory.SearchAsync(folder.Url, null).GetAwaiter().GetResult();

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
                    Description = String.Format("Read {0}", blobItem.Url)
                };
                progressCallback(progressInfo);

                var contentFile = new ContentFile
                {
                    Url = blobItem.RelativeUrl
                };
                using (var stream = _contentStorageProviderFactory.OpenRead(blobItem.Url))
                {
                    contentFile.Data = stream.ReadFully();
                }
                folder.Files.Add(contentFile);
            }
        }
    }
}

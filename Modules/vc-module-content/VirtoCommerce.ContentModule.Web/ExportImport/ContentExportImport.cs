using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ContentModule.Web.ExportImport
{
    public sealed class ContentExportImport
    {
        private static string[] _exportedFolders = { "Pages", "Themes" };
        private readonly IMenuService _menuService;
        private readonly IContentBlobStorageProvider _contentStorageProvider;

        public ContentExportImport(IMenuService menuService, Func<string, IContentBlobStorageProvider> themesStorageProviderFactory)
        {
            if (themesStorageProviderFactory == null)
                throw new ArgumentNullException(nameof(themesStorageProviderFactory));

            _menuService = menuService;
            _contentStorageProvider = themesStorageProviderFactory(string.Empty);
        }

        public void DoExport(Stream backupStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));

            var backupObject = GetBackupObject(progressCallback, manifest.HandleBinaryData);
            backupObject.SerializeJson(backupStream);
        }

        public void DoImport(Stream backupStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));

            var backupObject = backupStream.DeserializeJson<BackupObject>();
            var originalObject = GetBackupObject(progressCallback, false);

            var progressInfo = new ExportImportProgressInfo
            {
                Description = String.Format("{0} menu link lists importing...", backupObject.MenuLinkLists.Count())
            };
            progressCallback(progressInfo);
            UpdateMenuLinkLists(backupObject.MenuLinkLists);

            if (manifest.HandleBinaryData)
            {
                progressInfo.Description = String.Format("importing binary data:  themes and pages importing...");
                progressCallback(progressInfo);
                foreach (var folder in backupObject.ContentFolders)
                {
                    SaveContentFolderRecursive(folder, progressCallback);
                }
            }
        }

        private void UpdateMenuLinkLists(ICollection<webModels.MenuLinkList> linkLIsts)
        {
            foreach (var item in linkLIsts.Select(x => x.ToCoreModel()))
            {
                _menuService.AddOrUpdate(item);
            }
        }

        private BackupObject GetBackupObject(Action<ExportImportProgressInfo> progressCallback, bool handleBynaryData)
        {
            var retVal = new BackupObject();

            var progressInfo = new ExportImportProgressInfo
            {
                Description = "cms content loading..."
            };
            progressCallback(progressInfo);

            retVal.MenuLinkLists = _menuService.GetAllLinkLists().Select(x => x.ToWebModel()).ToList();

            if (handleBynaryData)
            {
                var result = _contentStorageProvider.Search("", null);
                foreach (var blobFolder in result.Folders.Where(x => _exportedFolders.Contains(x.Name)))
                {
                    var contentFolder = new ContentFolder
                    {
                        Url = blobFolder.RelativeUrl
                    };
                    ReadContentFoldersRecurive(contentFolder, progressCallback);
                    retVal.ContentFolders.Add(contentFolder);
                }
            }

            return retVal;
        }

        private void SaveContentFolderRecursive(ContentFolder folder, Action<ExportImportProgressInfo> progressCallback)
        {
            foreach (var childFolder in folder.Folders)
            {
                SaveContentFolderRecursive(childFolder, progressCallback);
            }
            foreach (var folderFile in folder.Files)
            {
                using (var stream = _contentStorageProvider.OpenWrite(folderFile.Url))
                using (var memStream = new MemoryStream(folderFile.Data))
                {
                    var progressInfo = new ExportImportProgressInfo
                    {
                        Description = String.Format("Saving {0}", folderFile.Url)
                    };
                    progressCallback(progressInfo);
                    memStream.CopyTo(stream);
                }
            }
        }

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
                    Description = String.Format("Read {0}", blobItem.Url)
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

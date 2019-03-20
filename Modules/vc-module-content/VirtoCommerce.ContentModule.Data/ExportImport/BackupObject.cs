using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.ExportImport
{
    public class ContentFolder
    {
        public ContentFolder()
        {
            Folders = new List<ContentFolder>();
            Files = new List<ContentFile>();
        }
        public string Url { get; set; }
        public ICollection<ContentFolder> Folders { get; set; }
        public ICollection<ContentFile> Files { get; set; }
    }
    public class ContentFile
    {
        public string Url { get; set; }
        public byte[] Data { get; set; }
    }

    public sealed class BackupObject
    {
        public BackupObject()
        {
            MenuLinkLists = new List<MenuLinkList>();
            ContentFolders = new List<ContentFolder>();
        }
        public ICollection<MenuLinkList> MenuLinkLists { get; set; }
        public ICollection<ContentFolder> ContentFolders { get; set; }
    }
}

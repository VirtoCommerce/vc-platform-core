using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Web.ExportImport
{
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

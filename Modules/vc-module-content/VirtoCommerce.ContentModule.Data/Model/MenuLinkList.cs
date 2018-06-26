using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class MenuLinkList : AuditableEntity
    {
        public MenuLinkList()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string StoreId { get; set; }

        public string Language { get; set; }
        public virtual ObservableCollection<MenuLink> MenuLinks { get; set; } = new ObservableCollection<MenuLink>();

    }
}

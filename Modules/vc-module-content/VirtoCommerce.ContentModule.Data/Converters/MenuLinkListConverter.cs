using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ContentModule.Data.Converters;

namespace VirtoCommerce.ContentModule.Data.Converters
{
    public static class MenuLinkListConverter
    {
        public static MenuLinkListEntity FromModel( this MenuLinkList list)
        {
            var listEntity = AbstractTypeFactory<MenuLinkListEntity>.TryCreateInstance();

            listEntity.Id = list.Id;
            listEntity.StoreId = list.StoreId;
            listEntity.Name = list.Name;
            listEntity.Language = list.Language;
            listEntity.ModifiedDate = list.ModifiedDate;
            listEntity.ModifiedBy = list.ModifiedBy;

            foreach (var link in list.MenuLinks)
            {
                listEntity.MenuLinks.Add(link.FromModel());
            }

            return listEntity;
        }
    }
}

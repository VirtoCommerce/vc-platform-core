using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentItemEntity : AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// available values in DynamicContentType enum
        /// </summary>
        [StringLength(64)]
        public string ContentTypeId { get; set; }

        public bool IsMultilingual { get; set; }

        [StringLength(2048)]
        public string ImageUrl { get; set; }

        #region Navigation Properties

        public string FolderId { get; set; }
        public virtual DynamicContentFolderEntity Folder { get; set; }

        public ObservableCollection<DynamicContentItemDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; set; }
            = new NullCollection<DynamicContentItemDynamicPropertyObjectValueEntity>();

        #endregion

        public virtual DynamicContentItem ToModel(DynamicContentItem item)
        {
            if (item == null)
                throw new NullReferenceException(nameof(item));

            item.Id = Id;
            item.CreatedBy = CreatedBy;
            item.CreatedDate = CreatedDate;
            item.ModifiedBy = ModifiedBy;
            item.ModifiedDate = ModifiedDate;

            item.Name = Name;
            item.FolderId = FolderId;
            item.ImageUrl = ImageUrl;
            item.ContentType = ContentTypeId;
            item.Description = Description;

            if (Folder != null)
            {
                item.Folder = Folder.ToModel(AbstractTypeFactory<DynamicContentFolder>.TryCreateInstance());
            }

            item.DynamicProperties = DynamicPropertyObjectValues.GroupBy(g => g.PropertyId).Select(x =>
            {
                var property = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                property.Id = x.Key;
                property.Name = x.FirstOrDefault()?.PropertyName;
                property.Values = x.Select(v => v.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance())).ToArray();
                return property;
            }).ToArray();

            return item;
        }

        public virtual DynamicContentItemEntity FromModel(DynamicContentItem item, PrimaryKeyResolvingMap pkMap)
        {
            if (item == null)
                throw new NullReferenceException(nameof(item));

            pkMap.AddPair(item, this);

            Id = item.Id;
            CreatedBy = item.CreatedBy;
            CreatedDate = item.CreatedDate;
            ModifiedBy = item.ModifiedBy;
            ModifiedDate = item.ModifiedDate;

            Name = item.Name;
            FolderId = item.FolderId;
            ImageUrl = item.ImageUrl;
            ContentTypeId = item.ContentType;
            Description = item.Description;

            if (item.DynamicProperties != null)
            {
                ContentTypeId = item.GetDynamicPropertyValue<string>("Content type", null);
                DynamicPropertyObjectValues = new ObservableCollection<DynamicContentItemDynamicPropertyObjectValueEntity>(item.DynamicProperties.SelectMany(p => p.Values
                    .Select(v => AbstractTypeFactory<DynamicContentItemDynamicPropertyObjectValueEntity>.TryCreateInstance().FromModel(v, item, p))).OfType<DynamicContentItemDynamicPropertyObjectValueEntity>());
            }

            return this;
        }

        public virtual void Patch(DynamicContentItemEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.Name = Name;
            target.Description = Description;
            target.FolderId = FolderId;
            target.ContentTypeId = ContentTypeId;
            target.ImageUrl = ImageUrl;

            if (!DynamicPropertyObjectValues.IsNullCollection())
            {
                DynamicPropertyObjectValues.Patch(target.DynamicPropertyObjectValues, (sourceDynamicPropertyObjectValues, targetDynamicPropertyObjectValues) => sourceDynamicPropertyObjectValues.Patch(targetDynamicPropertyObjectValues));
            }
        }
    }
}

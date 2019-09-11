using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentFolderEntity : AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(2048)]
        public string ImageUrl { get; set; }

        #region Navigation Properties

        public string ParentFolderId { get; set; }
        public virtual DynamicContentFolderEntity ParentFolder { get; set; }

        public virtual ObservableCollection<DynamicContentItemEntity> ContentItems { get; set; }
            = new NullCollection<DynamicContentItemEntity>();

        public virtual ObservableCollection<DynamicContentPlaceEntity> ContentPlaces { get; set; }
            = new NullCollection<DynamicContentPlaceEntity>();

        #endregion

        public virtual DynamicContentFolder ToModel(DynamicContentFolder folder)
        {
            if (folder == null)
                throw new NullReferenceException(nameof(folder));

            folder.Id = Id;
            folder.CreatedBy = CreatedBy;
            folder.CreatedDate = CreatedDate;
            folder.ModifiedBy = ModifiedBy;
            folder.ModifiedDate = ModifiedDate;

            folder.Name = Name;
            folder.ParentFolderId = ParentFolderId;
            folder.Description = Description;

            if (ParentFolder != null)
            {
                folder.ParentFolder = ParentFolder.ToModel(AbstractTypeFactory<DynamicContentFolder>.TryCreateInstance());
            }
            return folder;
        }

        public virtual DynamicContentFolderEntity FromModel(DynamicContentFolder folder, PrimaryKeyResolvingMap pkMap)
        {
            if (folder == null)
                throw new NullReferenceException(nameof(folder));

            pkMap.AddPair(folder, this);

            Id = folder.Id;
            CreatedBy = folder.CreatedBy;
            CreatedDate = folder.CreatedDate;
            ModifiedBy = folder.ModifiedBy;
            ModifiedDate = folder.ModifiedDate;

            Name = folder.Name;
            ParentFolderId = folder.ParentFolderId;
            Description = folder.Description;

            return this;
        }

        public virtual void Patch(DynamicContentFolderEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.Name = Name;
            target.Description = Description;
        }        
    }
}

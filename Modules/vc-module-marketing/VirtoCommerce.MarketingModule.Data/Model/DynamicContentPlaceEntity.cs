using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentPlaceEntity : AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(2048)]
        public string ImageUrl { get; set; }

        #region Navigation Properties

        public string FolderId { get; set; }
        public virtual DynamicContentFolderEntity Folder { get; set; }

        #endregion

        public virtual DynamicContentPlace ToModel(DynamicContentPlace place)
        {
            if (place == null)
                throw new NullReferenceException(nameof(place));

            place.Id = Id;
            place.CreatedBy = CreatedBy;
            place.CreatedDate = CreatedDate;
            place.ModifiedBy = ModifiedBy;
            place.ModifiedDate = ModifiedDate;

            place.Name = Name;
            place.FolderId = FolderId;
            place.ImageUrl = ImageUrl;
            place.Description = Description;

            if (Folder != null)
            {
                place.Folder = Folder.ToModel(AbstractTypeFactory<DynamicContentFolder>.TryCreateInstance());
            }

            return place;
        }

        public virtual DynamicContentPlaceEntity FromModel(DynamicContentPlace place, PrimaryKeyResolvingMap pkMap)
        {
            if (place == null)
                throw new NullReferenceException(nameof(place));

            pkMap.AddPair(place, this);

            Id = place.Id;
            CreatedBy = place.CreatedBy;
            CreatedDate = place.CreatedDate;
            ModifiedBy = place.ModifiedBy;
            ModifiedDate = place.ModifiedDate;

            Name = place.Name;
            FolderId = place.FolderId;
            ImageUrl = place.ImageUrl;
            Description = place.Description;

            return this;
        }

        public virtual void Patch(DynamicContentPlaceEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.Name = Name;
            target.Description = Description;
            target.FolderId = FolderId;
            target.ImageUrl = ImageUrl;
        }
    }
}

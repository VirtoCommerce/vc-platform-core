using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyAttributeEntity : AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string PropertyAttributeName { get; set; }

        [Required]
        [StringLength(128)]
        public string PropertyAttributeValue { get; set; }

        public int Priority { get; set; }

        #region Navigation Properties

        public string PropertyId { get; set; }
        public virtual PropertyEntity Property { get; set; }

        #endregion

        public virtual PropertyAttribute ToModel(PropertyAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            attribute.Id = Id;
            attribute.CreatedBy = CreatedBy;
            attribute.CreatedDate = CreatedDate;
            attribute.ModifiedBy = ModifiedBy;
            attribute.ModifiedDate = ModifiedDate;

            attribute.Name = PropertyAttributeName;
            attribute.Value = PropertyAttributeValue;
            attribute.PropertyId = PropertyId;

            return attribute;
        }

        public virtual PropertyAttributeEntity FromModel(PropertyAttribute attribute, PrimaryKeyResolvingMap pkMap)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            pkMap.AddPair(attribute, this);

            Id = attribute.Id;
            CreatedBy = attribute.CreatedBy;
            CreatedDate = attribute.CreatedDate;
            ModifiedBy = attribute.ModifiedBy;
            ModifiedDate = attribute.ModifiedDate;

            PropertyId = attribute.PropertyId;
            PropertyAttributeName = attribute.Name;
            PropertyAttributeValue = attribute.Value;

            return this;
        }

        public virtual void Patch(PropertyAttributeEntity target)
        {
            target.PropertyAttributeName = PropertyAttributeName;
            target.PropertyAttributeValue = PropertyAttributeValue;
        }
    }
}

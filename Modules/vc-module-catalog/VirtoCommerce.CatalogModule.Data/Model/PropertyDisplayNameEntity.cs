using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyDisplayNameEntity : Entity
    {
        [StringLength(64)]
        public string Locale { get; set; }
        [StringLength(512)]
        public string Name { get; set; }

        #region Navigation Properties

        public string PropertyId { get; set; }
        public virtual PropertyEntity Property { get; set; }

        #endregion

        public virtual PropertyDisplayName ToModel(PropertyDisplayName displayName)
        {
            if (displayName == null)
                throw new ArgumentNullException(nameof(displayName));

            displayName.LanguageCode = Locale;
            displayName.Name = Name;

            return displayName;
        }

        public virtual PropertyDisplayNameEntity FromModel(PropertyDisplayName displayName)
        {
            if (displayName == null)
                throw new ArgumentNullException(nameof(displayName));

            Locale = displayName.LanguageCode;
            Name = displayName.Name;

            return this;
        }

        public virtual void Patch(PropertyDisplayNameEntity target)
        {

        }
    }
}

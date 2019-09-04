using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyValidationRuleEntity : Entity
    {
        public bool IsUnique { get; set; }

        public int? CharCountMin { get; set; }

        public int? CharCountMax { get; set; }

        [StringLength(2048)]
        public string RegExp { get; set; }

        #region Navigation properties

        public string PropertyId { get; set; }
        public PropertyEntity Property { get; set; }

        #endregion

        public virtual PropertyValidationRule ToModel(PropertyValidationRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            rule.Id = Id;
            rule.CharCountMax = CharCountMax;
            rule.CharCountMin = CharCountMin;
            rule.IsUnique = IsUnique;
            rule.RegExp = RegExp;
            rule.PropertyId = PropertyId;

            return rule;
        }

        public virtual PropertyValidationRuleEntity FromModel(PropertyValidationRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            Id = rule.Id;
            CharCountMax = rule.CharCountMax;
            CharCountMin = rule.CharCountMin;
            IsUnique = rule.IsUnique;
            RegExp = rule.RegExp;
            PropertyId = rule.PropertyId;

            return this;
        }

        public virtual void Patch(PropertyValidationRuleEntity target)
        {
            target.CharCountMax = CharCountMax;
            target.CharCountMin = CharCountMin;
            target.IsUnique = IsUnique;
            target.RegExp = RegExp;
        }
    }
}

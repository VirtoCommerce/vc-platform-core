using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class PropertyEntity : AuditableEntity
    {
        public PropertyEntity()
        {
            DictionaryItems = new NullCollection<PropertyDictionaryItemEntity>();
            PropertyAttributes = new NullCollection<PropertyAttributeEntity>();
            DisplayNames = new NullCollection<PropertyDisplayNameEntity>();
            ValidationRules = new NullCollection<PropertyValidationRuleEntity>();
        }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(128)]
        public string TargetType { get; set; }

        public bool IsKey { get; set; }

        public bool IsSale { get; set; }

        public bool IsEnum { get; set; }

        public bool IsInput { get; set; }

        public bool IsRequired { get; set; }

        public bool IsMultiValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locale dependant. If true, the locale must be specified for the values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is locale dependant; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocaleDependant { get; set; }

        public bool AllowAlias { get; set; }

        [Required]
        public int PropertyValueType { get; set; }

        #region Navigation Properties

        public string CatalogId { get; set; }
        public virtual CatalogEntity Catalog { get; set; }

        public string CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        public virtual ObservableCollection<PropertyDictionaryItemEntity> DictionaryItems { get; set; }
        public virtual ObservableCollection<PropertyAttributeEntity> PropertyAttributes { get; set; }
        public virtual ObservableCollection<PropertyDisplayNameEntity> DisplayNames { get; set; }
        public virtual ObservableCollection<PropertyValidationRuleEntity> ValidationRules { get; set; }

        #endregion

        public virtual Property ToModel(Property property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            property.Id = Id;
            property.CreatedBy = CreatedBy;
            property.CreatedDate = CreatedDate;
            property.ModifiedBy = ModifiedBy;
            property.ModifiedDate = ModifiedDate;

            property.CatalogId = CatalogId;
            property.CategoryId = CategoryId;

            property.Name = Name;
            property.Required = IsRequired;
            property.Multivalue = IsMultiValue;
            property.Multilanguage = IsLocaleDependant;
            property.Dictionary = IsEnum;
            property.ValueType = (PropertyValueType)PropertyValueType;
            property.Type = EnumUtility.SafeParseFlags(TargetType, PropertyType.Catalog);


            property.Attributes = PropertyAttributes.Select(x => x.ToModel(AbstractTypeFactory<PropertyAttribute>.TryCreateInstance())).ToList();
            property.DisplayNames = DisplayNames.Select(x => x.ToModel(AbstractTypeFactory<PropertyDisplayName>.TryCreateInstance())).ToList();
            property.ValidationRules = ValidationRules.Select(x => x.ToModel(AbstractTypeFactory<PropertyValidationRule>.TryCreateInstance())).ToList();

            foreach (var rule in property.ValidationRules)
            {
                rule.Property = property;
            }

            return property;
        }

        public virtual PropertyEntity FromModel(Property property, PrimaryKeyResolvingMap pkMap)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            pkMap.AddPair(property, this);

            Id = property.Id;
            CreatedBy = property.CreatedBy;
            CreatedDate = property.CreatedDate;
            ModifiedBy = property.ModifiedBy;
            ModifiedDate = property.ModifiedDate;

            CatalogId = property.CatalogId;
            CategoryId = property.CategoryId;

            Name = property.Name;
            PropertyValueType = (int)property.ValueType;
            IsMultiValue = property.Multivalue;
            IsLocaleDependant = property.Multilanguage;
            IsEnum = property.Dictionary;
            IsRequired = property.Required;
            TargetType = property.Type.ToString();

            if (property.Attributes != null)
            {
                PropertyAttributes = new ObservableCollection<PropertyAttributeEntity>(property.Attributes.Select(x => AbstractTypeFactory<PropertyAttributeEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (property.DisplayNames != null)
            {
                DisplayNames = new ObservableCollection<PropertyDisplayNameEntity>(property.DisplayNames.Select(x => AbstractTypeFactory<PropertyDisplayNameEntity>.TryCreateInstance().FromModel(x)));
            }

            if (property.ValidationRules != null)
            {
                ValidationRules = new ObservableCollection<PropertyValidationRuleEntity>(property.ValidationRules.Select(x => AbstractTypeFactory<PropertyValidationRuleEntity>.TryCreateInstance().FromModel(x)));
            }
            return this;
        }

        public virtual void Patch(PropertyEntity target)
        {
            target.PropertyValueType = PropertyValueType;
            target.IsEnum = IsEnum;
            target.IsMultiValue = IsMultiValue;
            target.IsLocaleDependant = IsLocaleDependant;
            target.IsRequired = IsRequired;
            target.TargetType = TargetType;
            target.Name = Name;

            target.CatalogId = CatalogId;
            target.CategoryId = CategoryId;

            if (!PropertyAttributes.IsNullCollection())
            {
                var attributeComparer = AnonymousComparer.Create((PropertyAttributeEntity x) => x.IsTransient() ? x.PropertyAttributeName : x.Id);
                PropertyAttributes.Patch(target.PropertyAttributes, attributeComparer, (sourceAsset, targetAsset) => sourceAsset.Patch(targetAsset));
            }
            if (!DictionaryItems.IsNullCollection())
            {
                var dictItemComparer = AnonymousComparer.Create((PropertyDictionaryItemEntity x) => $"{x.Alias}-${x.PropertyId}");
                DictionaryItems.Patch(target.DictionaryItems, dictItemComparer, (sourceDictItem, targetDictItem) => sourceDictItem.Patch(targetDictItem));
            }
            if (!DisplayNames.IsNullCollection())
            {
                var displayNamesComparer = AnonymousComparer.Create((PropertyDisplayNameEntity x) => $"{x.Name}-{x.Locale}");
                DisplayNames.Patch(target.DisplayNames, displayNamesComparer, (sourceDisplayName, targetDisplayName) => sourceDisplayName.Patch(targetDisplayName));
            }

            if (!ValidationRules.IsNullCollection())
            {
                ValidationRules.Patch(target.ValidationRules, (sourceRule, targetRule) => sourceRule.Patch(targetRule));
            }
        }
    }
}

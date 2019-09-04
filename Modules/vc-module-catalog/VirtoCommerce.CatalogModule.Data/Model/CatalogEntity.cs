using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CatalogEntity : AuditableEntity, IHasOuterId
    {
        public bool Virtual { get; set; }
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(64)]
        [Required]
        public string DefaultLanguage { get; set; }

        [StringLength(128)]
        public string OwnerId { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<CategoryRelationEntity> IncomingLinks { get; set; }
            = new NullCollection<CategoryRelationEntity>();

        public virtual ObservableCollection<CatalogLanguageEntity> CatalogLanguages { get; set; }
            = new NullCollection<CatalogLanguageEntity>();

        public virtual ObservableCollection<PropertyValueEntity> CatalogPropertyValues { get; set; }
            = new NullCollection<PropertyValueEntity>();

        public virtual ObservableCollection<PropertyEntity> Properties { get; set; }
            = new NullCollection<PropertyEntity>();

        #endregion

        public virtual Catalog ToModel(Catalog catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            catalog.Id = Id;
            catalog.Name = Name;
            catalog.IsVirtual = Virtual;
            catalog.OuterId = OuterId;

            catalog.Languages = new List<CatalogLanguage>();
            var defaultLanguage = (new CatalogLanguageEntity { Language = string.IsNullOrEmpty(DefaultLanguage) ? "en-us" : DefaultLanguage }).ToModel(AbstractTypeFactory<CatalogLanguage>.TryCreateInstance());
            defaultLanguage.IsDefault = true;
            catalog.Languages.Add(defaultLanguage);
            //populate additional languages
            foreach (var additionalLanguage in CatalogLanguages.Where(x => x.Language != defaultLanguage.LanguageCode).Select(x => x.ToModel(AbstractTypeFactory<CatalogLanguage>.TryCreateInstance())))
            {
                catalog.Languages.Add(additionalLanguage);
            }

            //Self properties
            catalog.Properties = Properties.Where(x => x.CategoryId == null)
                .OrderBy(x => x.Name)
                .Select(x => x.ToModel(AbstractTypeFactory<Property>.TryCreateInstance())).ToList();


            foreach (var property in catalog.Properties)
            {
                property.IsReadOnly = property.Type != PropertyType.Catalog;
                property.Values = CatalogPropertyValues.Where(pr => pr.Name.EqualsInvariant(property.Name)).OrderBy(x => x.DictionaryItem?.SortOrder)
                    .ThenBy(x => x.Name)
                    .SelectMany(x => x.ToModel(AbstractTypeFactory<PropertyValue>.TryCreateInstance())).ToList();
            }

            return catalog;
        }

        public virtual CatalogEntity FromModel(Catalog catalog, PrimaryKeyResolvingMap pkMap)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            if (catalog.DefaultLanguage == null)
            {
                throw new NullReferenceException("DefaultLanguage");
            }

            pkMap.AddPair(catalog, this);

            Id = catalog.Id;
            Name = catalog.Name;
            Virtual = catalog.IsVirtual;
            OuterId = catalog.OuterId;

            DefaultLanguage = catalog.DefaultLanguage.LanguageCode;

            if (!catalog.Properties.IsNullOrEmpty())
            {
                var propValues = new List<PropertyValue>();
                foreach (var property in catalog.Properties)
                {
                    if (property.Values != null)
                    {
                        foreach (var propValue in property.Values)
                        {
                            //Need populate required fields
                            propValue.PropertyName = property.Name;
                            propValue.ValueType = property.ValueType;
                            propValues.Add(propValue);
                        }
                    }
                }
                if (!propValues.IsNullOrEmpty())
                {
                    CatalogPropertyValues = new ObservableCollection<PropertyValueEntity>(AbstractTypeFactory<PropertyValueEntity>.TryCreateInstance().FromModels(propValues, pkMap));
                }
            }

            if (catalog.Languages != null)
            {
                CatalogLanguages = new ObservableCollection<CatalogLanguageEntity>(catalog.Languages.Select(x => AbstractTypeFactory<CatalogLanguageEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            return this;
        }

        public virtual void Patch(CatalogEntity target)
        {
            target.Name = Name;
            target.DefaultLanguage = DefaultLanguage;

            //Languages patch
            if (!CatalogLanguages.IsNullCollection())
            {
                var languageComparer = AnonymousComparer.Create((CatalogLanguageEntity x) => x.Language);
                CatalogLanguages.Patch(target.CatalogLanguages, languageComparer,
                                                     (sourceLang, targetlang) => sourceLang.Patch(targetlang));
            }

            //Property values
            if (!CatalogPropertyValues.IsNullCollection())
            {
                CatalogPropertyValues.Patch(target.CatalogPropertyValues, (sourcePropValue, targetPropValue) => sourcePropValue.Patch(targetPropValue));
            }
        }
    }
}

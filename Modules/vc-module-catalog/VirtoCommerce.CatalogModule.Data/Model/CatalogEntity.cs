using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class CatalogEntity : AuditableEntity
    {
        public CatalogEntity()
        {

            CatalogLanguages = new NullCollection<CatalogLanguageEntity>();
            CatalogPropertyValues = new NullCollection<PropertyValueEntity>();
            IncommingLinks = new NullCollection<CategoryRelationEntity>();
            Properties = new NullCollection<PropertyEntity>();
        }

        public bool Virtual { get; set; }
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(64)]
        [Required]
        public string DefaultLanguage { get; set; }

        [StringLength(128)]
        public string OwnerId { get; set; }

        #region Navigation Properties
        public virtual ObservableCollection<CategoryRelationEntity> IncommingLinks { get; set; }

        public virtual ObservableCollection<CatalogLanguageEntity> CatalogLanguages { get; set; }
        public virtual ObservableCollection<PropertyValueEntity> CatalogPropertyValues { get; set; }
        public virtual ObservableCollection<PropertyEntity> Properties { get; set; }
        #endregion


        public virtual Catalog ToModel(Catalog catalog)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            catalog.Id = Id;
            catalog.Name = Name;
            catalog.IsVirtual = Virtual;

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
                property.Values = CatalogPropertyValues.Where(pr => pr.Name.EqualsInvariant(property.Name)).OrderBy(x => x.DictionaryItem?.SortOrder)
                    .ThenBy(x => x.Name)
                    .SelectMany(x => x.ToModel(AbstractTypeFactory<PropertyValue>.TryCreateInstance())).ToList();
            }

            ////item property values
            //catalog.PropertyValues = CatalogPropertyValues
            //    .OrderBy(x => x.DictionaryItem?.SortOrder)
            //    .ThenBy(x => x.Name)
            //    .SelectMany(x => x.ToModel(AbstractTypeFactory<PropertyValue>.TryCreateInstance())).ToList();



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
            DefaultLanguage = catalog.DefaultLanguage.LanguageCode;

            if (!catalog.Properties.IsNullOrEmpty())
            {
                CatalogPropertyValues = new ObservableCollection<PropertyValueEntity>(
                    AbstractTypeFactory<PropertyValueEntity>.TryCreateInstance()
                        .FromModels(catalog.Properties.SelectMany(pr => pr.Values), pkMap));
                //var propertyValues = new ObservableCollection<PropertyValueEntity>();
                //foreach (var property in catalog.Properties)
                //{
                //    propertyValues.AddRange(new ObservableCollection<PropertyValueEntity>(
                //        AbstractTypeFactory<PropertyValueEntity>
                //            .TryCreateInstance()
                //            .FromModels(catalog.Properties.SelectMany(pv => pv.Values), property, pkMap)));
                //}
                //CatalogPropertyValues = propertyValues;
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

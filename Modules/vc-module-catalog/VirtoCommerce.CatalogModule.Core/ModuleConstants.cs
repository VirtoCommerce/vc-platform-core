using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CatalogModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string CatalogAccess = "catalog:access",
                  CatalogCreate = "catalog:create",
                  CatalogRead = "catalog:read",
                  CatalogUpdate = "catalog:update",
                  CatalogDelete = "catalog:delete",
                  CatalogExport = "catalog:export",
                  CatalogImport = "catalog:import";

                public static string[] AllPermissions = new[] { CatalogAccess, CatalogCreate, CatalogRead, CatalogUpdate, CatalogDelete, CatalogExport, CatalogImport };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor AssociationGroups = new SettingDescriptor
                {
                    Name = "Catalog.AssociationGroups",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Catalog|General",
                    IsDictionary = true,
                    AllowedValues = new string[] { "Accessories", "Related Items" }
                };

                public static SettingDescriptor EditorialReviewTypes = new SettingDescriptor
                {
                    Name = "Catalog.EditorialReviewTypes",
                    ValueType = SettingValueType.ShortText,
                    GroupName = "Catalog|General",
                    IsDictionary = true,
                    DefaultValue = "QuickReview",
                    AllowedValues = new string[] { "QuickReview", "FullReview" }
                };

                public static SettingDescriptor CodesInOutline = new SettingDescriptor
                {
                    Name = "Catalog.CodesInOutline",
                    GroupName = "Catalog|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };
                public static SettingDescriptor ExposeAliasInDictionary = new SettingDescriptor
                {
                    Name = "Catalog.ExposeAliasInDictionary",
                    GroupName = "Catalog|General",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return AssociationGroups;
                        yield return EditorialReviewTypes;
                        yield return CodesInOutline;
                        yield return ExposeAliasInDictionary;
                    }
                }
            }

            public static class Search
            {
                public static SettingDescriptor UseCatalogIndexedSearchInManager = new SettingDescriptor
                {
                    Name = "Catalog.Search.UseCatalogIndexedSearchInManager",
                    GroupName = "Catalog|Search",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true
                };

                public static SettingDescriptor UseFullObjectIndexStoring = new SettingDescriptor
                {
                    Name = "Catalog.Search.UseFullObjectIndexStoring",
                    GroupName = "Catalog|Search",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = false
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return UseCatalogIndexedSearchInManager;
                        yield return UseFullObjectIndexStoring;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings.Concat(Search.AllSettings);
                }
            }
        }
    }
}

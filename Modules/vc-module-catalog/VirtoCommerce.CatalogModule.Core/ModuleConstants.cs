using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Modularity;

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
                public static ModuleSetting AssociationGroups = new ModuleSetting
                {
                    Name = "Catalog.AssociationGroups",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    ArrayValues = new string[] { "Accessories", "Related Items" }
                };

                public static ModuleSetting EditorialReviewTypes = new ModuleSetting
                {
                    Name = "Catalog.EditorialReviewTypes",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    DefaultValue = "QuickReview",
                    ArrayValues = new string[] { "QuickReview", "FullReview" }
                };

                public static ModuleSetting CodesInOutline = new ModuleSetting
                {
                    Name = "Catalog.CodesInOutline",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString()
                };
                public static ModuleSetting ExposeAliasInDictionary = new ModuleSetting
                {
                    Name = "Catalog.ExposeAliasInDictionary",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString()
                };

                public static IEnumerable<ModuleSetting> AllSettings
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
                public static ModuleSetting UseCatalogIndexedSearchInManager = new ModuleSetting
                {
                    Name = "Catalog.Search.UseCatalogIndexedSearchInManager",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = true.ToString()
                };

                public static ModuleSetting UseFullObjectIndexStoring = new ModuleSetting
                {
                    Name = "Catalog.Search.UseFullObjectIndexStoring",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString()
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return UseCatalogIndexedSearchInManager;
                        yield return UseFullObjectIndexStoring;
                    }
                }
            }
        }
    }
}

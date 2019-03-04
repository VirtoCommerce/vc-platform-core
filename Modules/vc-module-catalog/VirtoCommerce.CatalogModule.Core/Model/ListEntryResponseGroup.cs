using System;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    [Flags]
    public enum ListEntryResponseGroup
    {
        WithProducts = 1,
        WithCategories = 2,
        WithProperties = 4,
        WithCatalogs = 8,
        WithVariations = 16,
        Full = WithCatalogs | WithCategories | WithProperties | WithProducts | WithVariations
    }
}

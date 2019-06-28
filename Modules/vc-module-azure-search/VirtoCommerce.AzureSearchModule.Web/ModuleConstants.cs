using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.AzureSearchModule.Web
{
    public class ModuleConstants
    {
        public static class Settings
        {
            public static class Indexing
            {
                private static readonly SettingDescriptor TokenFilter = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.AzureSearch.TokenFilter",
                    GroupName = "Search|Azure Search",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "custom_edge_ngram",
                    AllowedValues = new object[] { "custom_edge_ngram", "custom_ngram" }
                };

                private static readonly SettingDescriptor MinGram = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.AzureSearch.NGramTokenFilter.MinGram",
                    GroupName = "Search|Azure Search",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 1
                };

                private static readonly SettingDescriptor MaxGram = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.AzureSearch.NGramTokenFilter.MaxGram",
                    GroupName = "Search|Azure Search",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 20
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return TokenFilter;
                        yield return MinGram;
                        yield return MaxGram;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => Indexing.AllSettings;
        }
    }
}


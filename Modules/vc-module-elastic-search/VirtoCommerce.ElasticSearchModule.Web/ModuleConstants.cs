using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ElasticSearchModule.Web
{
    public static class ModuleConstants
    {
        public static class Settings
        {
            public static class Indexing
            {
                private static readonly SettingDescriptor IndexTotalFieldsLimit = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.ElasticSearch.IndexTotalFieldsLimit",
                    GroupName = "Platform|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 1000
                };

                private static readonly SettingDescriptor TokenFilter = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.ElasticSearch.TokenFilter",
                    GroupName = "Platform|General",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "custom_edge_ngram"
                };

                private static readonly SettingDescriptor MinGram = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.ElasticSearch.NGramTokenFilter.MinGram",
                    GroupName = "Platform|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 1
                };

                private static readonly SettingDescriptor MaxGram = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.ElasticSearch.NGramTokenFilter.MaxGram",
                    GroupName = "Platform|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 20
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return IndexTotalFieldsLimit;
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

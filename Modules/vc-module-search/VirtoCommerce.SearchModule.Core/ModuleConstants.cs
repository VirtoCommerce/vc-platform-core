using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.SearchModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string IndexRebuild = "search:index:rebuild";

                public static string[] AllPermissions = new[] {IndexRebuild};
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static ModuleSetting SearchConnectionString = new ModuleSetting
                {
                    Name = "VirtoCommerce.Search.SearchConnectionString",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "provider=Lucene;server=~/App_Data/Lucene;scope=default",
                    RestartRequired = true
                };

                public static ModuleSetting IndexPartitionSize = new ModuleSetting
                {
                    Name = "VirtoCommerce.Search.IndexPartitionSize",
                    ValueType = ModuleSetting.TypeInteger,
                    DefaultValue = "50",
                };

                public static class IndexingJobs
                {
                    public static ModuleSetting Enable = new ModuleSetting
                    {
                        Name = "VirtoCommerce.Search.IndexingJobs.Enable",
                        ValueType = ModuleSetting.TypeBoolean,
                        DefaultValue = true.ToString(),
                        RestartRequired = true
                    };

                    public static ModuleSetting CronExpression = new ModuleSetting
                    {
                        Name = "VirtoCommerce.Search.IndexingJobs.CronExpression",
                        ValueType = ModuleSetting.TypeString,
                        DefaultValue = "0/5 * * * *",
                        RestartRequired = true
                    };

                    public static ModuleSetting ScaleOut = new ModuleSetting
                    {
                        Name = "VirtoCommerce.Search.IndexingJobs.ScaleOut",
                        ValueType = ModuleSetting.TypeBoolean,
                        DefaultValue = false.ToString(),
                        RestartRequired = true
                    };

                    public static ModuleSetting MaxQueueSize = new ModuleSetting
                    {
                        Name = "VirtoCommerce.Search.IndexingJobs.MaxQueueSize",
                        ValueType = ModuleSetting.TypeInteger,
                        DefaultValue = "25",
                        RestartRequired = true
                    };
                }

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return SearchConnectionString;
                        yield return IndexPartitionSize;
                        yield return IndexingJobs.Enable;
                        yield return IndexingJobs.CronExpression;
                        yield return IndexingJobs.ScaleOut;
                        yield return IndexingJobs.MaxQueueSize;
                    }
                }
            }
            
        }
    }
}

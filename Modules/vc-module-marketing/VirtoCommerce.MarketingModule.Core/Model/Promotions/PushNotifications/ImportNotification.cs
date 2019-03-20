using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.MarketingModule.Core.Model.PushNotifications
{
    public class ImportNotification : PushNotification
    {
        public ImportNotification(string creator)
            : base(creator)
        {
            NotifyType = "CouponCsvImport";
            Errors = new List<string>();
        }

        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        [JsonProperty("processedCount")]
        public long ProcessedCount { get; set; }

        [JsonProperty("errorCount")]
        public long ErrorCount { get; set; }

        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }
    }
}

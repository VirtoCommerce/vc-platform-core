using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.SitemapsModule.Web.Model.PushNotifications
{
    /// <summary>
    /// 
    /// </summary>
    public class SitemapDownloadNotification : PushNotification
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="creator"></param>
        public SitemapDownloadNotification(string creator)
            : base(creator)
        {
            NotifyType = "SitemapDownload";
            Errors = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("processedCount")]
        public long ProcessedCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("errorCount")]
        public long ErrorCount => Errors?.Count ?? 0;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }
    }
}

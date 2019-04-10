using System;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapProgressInfo : ExportImportProgressInfo
    {
        public string StartDescriptionTemplate { get; set; }
        public string ProgressDescriptionTemplate { get; set; }
        public string EndDescriptionTemplate { get; set; }

        public Action<ExportImportProgressInfo> ProgressCallback { get; set; }

        public void Start()
        {
            ProcessedCount = 0;
            Description = string.Format(StartDescriptionTemplate, TotalCount);
            ProgressCallback?.Invoke(this);
        }

        public void Next()
        {
            ProcessedCount++;
            Description = string.Format(ProgressDescriptionTemplate, ProcessedCount, TotalCount);
            ProgressCallback?.Invoke(this);
        }

        public void End()
        {
            Description = string.Format(EndDescriptionTemplate, TotalCount);
            ProgressCallback?.Invoke(this);
        }
    }
}

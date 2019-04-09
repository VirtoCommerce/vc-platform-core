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

        public void ProgressStarted()
        {
            ProcessedCount = 0;
            Description = String.Format(StartDescriptionTemplate, TotalCount);
            ProgressCallback?.Invoke(this);
        }

        public void NextProgress()
        {
            ProcessedCount++;
            Description = String.Format(ProgressDescriptionTemplate, ProcessedCount, TotalCount);
            ProgressCallback?.Invoke(this);
        }

        public void ProgressEnded()
        {
            Description = String.Format(EndDescriptionTemplate, TotalCount);
            ProgressCallback?.Invoke(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public class PlatformImportExportRequest
    {
        public string FileUrl { get; set; }
        public bool HandleSecurity { get; set; }
        public bool HandleSettings { get; set; }
        public bool HandleBinaryData { get; set; }
        public string[] Modules { get; set; }
        public PlatformExportManifest ExportManifest { get; set; }
    }
}

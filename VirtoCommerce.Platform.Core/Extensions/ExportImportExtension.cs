using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.ExportImport.PushNotifications;

namespace VirtoCommerce.Platform.Core.Extensions
{
    public static class ExportImportExtension
    {
        public static void Path(this PlatformExportImportPushNotification target, ExportImportProgressInfo  sourse )
        {
            target.Description = sourse.Description;
            target.Errors = sourse.Errors;
            target.ProcessedCount = sourse.ProcessedCount;
            target.TotalCount = sourse.TotalCount;

        }

        public static PlatformExportManifest ToManifest(this PlatformImportExportRequest request)
        {
            var retVal = request.ExportManifest;

            retVal.HandleBinaryData =  request.HandleBinaryData;
            retVal.HandleSecurity = request.HandleSecurity;
            retVal.HandleSettings = request.HandleSettings;

            //Leave only selected modules
            retVal.Modules = retVal.Modules.Where(x => request.Modules != null && request.Modules.Contains(x.Id)).ToArray();
            return retVal;
        }
    }
}

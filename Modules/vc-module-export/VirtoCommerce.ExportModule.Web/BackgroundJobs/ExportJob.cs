using System;
using System.IO;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.ExportImport.PushNotifications;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.ExportModule.Web.BackgroundJobs
{
    public class ExportJob
    {
        private readonly IPushNotificationManager _pushNotificationManager;
        private readonly PlatformOptions _platformOptions;
        private readonly IDataExporter _dataExporter;

        public ExportJob(IDataExporter dataExporter,
            IPushNotificationManager pushNotificationManager,
            IOptions<PlatformOptions> platformOptions)
        {
            _dataExporter = dataExporter;
            _pushNotificationManager = pushNotificationManager;
            _platformOptions = platformOptions.Value;
        }

        public async Task ExportBackgroundAsync(ExportDataRequest request, PlatformExportPushNotification notification, IJobCancellationToken cancellationToken, PerformContext context)
        {
            void progressCallback(ExportImportProgressInfo x)
            {
                notification.Path(x);
                notification.JobId = context.BackgroundJob.Id;
                _pushNotificationManager.Send(notification);
            }

            try
            {
                var localTmpFolder = Path.GetFullPath(Path.Combine(_platformOptions.DefaultExportFolder));
                var fileName = Path.GetFileName(_platformOptions.DefaultExportFileName);

                if (request.ProviderConfig != null)
                {
                    fileName = Path.ChangeExtension(Path.GetFileName(_platformOptions.DefaultExportFileName), request.ProviderConfig.ExportedFileExtension);
                }

                var localTmpPath = Path.Combine(localTmpFolder, fileName);

                if (!Directory.Exists(localTmpFolder))
                {
                    Directory.CreateDirectory(localTmpFolder);
                }

                if (File.Exists(localTmpPath))
                {
                    File.Delete(localTmpPath);
                }

                //Import first to local tmp folder because Azure blob storage doesn't support some special file access mode 
                using (var stream = File.OpenWrite(localTmpPath))
                {
                    _dataExporter.Export(stream, request, progressCallback, new JobCancellationTokenWrapper(cancellationToken));
                    notification.DownloadUrl = $"api/export/download/{fileName}";
                }
            }
            catch (JobAbortedException)
            {
                //do nothing
            }
            catch (Exception ex)
            {
                notification.Errors.Add(ex.ExpandExceptionMessage());
            }
            finally
            {
                notification.Description = "Export finished";
                notification.Finished = DateTime.UtcNow;
                await _pushNotificationManager.SendAsync(notification);
            }
        }
    }
}

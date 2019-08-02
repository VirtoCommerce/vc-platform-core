using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExportModule.Core;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.ExportModule.Web.BackgroundJobs;
using VirtoCommerce.ExportModule.Web.Model;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.ExportImport.PushNotifications;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExportModule.Web.Controllers
{
    [Route("api/export")]
    public class ExportController : Controller
    {
        private readonly IEnumerable<Func<IExportProviderConfiguration, ExportedTypeColumnInfo[], IExportProvider>> _exportProviderFactories;
        private readonly IKnownExportTypesRegistrar _knownExportTypesRegistrar;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _pushNotificationManager;
        private readonly PlatformOptions _platformOptions;
        private readonly IKnownExportTypesResolver _knownExportTypesResolver;

        public ExportController(
            IEnumerable<Func<IExportProviderConfiguration, ExportedTypeColumnInfo[], IExportProvider>> exportProviderFactories,
            IKnownExportTypesRegistrar knownExportTypesRegistrar,
            IUserNameResolver userNameResolver,
            IPushNotificationManager pushNotificationManager,
            IOptions<PlatformOptions> platformOptions,
            IKnownExportTypesResolver knownExportTypesResolver)
        {
            _exportProviderFactories = exportProviderFactories;
            _knownExportTypesRegistrar = knownExportTypesRegistrar;
            _userNameResolver = userNameResolver;
            _pushNotificationManager = pushNotificationManager;
            _platformOptions = platformOptions.Value;
            _knownExportTypesResolver = knownExportTypesResolver;
        }

        /// <summary>
        /// Gets the list of types ready to be exported
        /// </summary>
        /// <returns>The list of exported known types</returns>
        [HttpGet]
        [Route("knowntypes")]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public ActionResult<ExportedTypeDefinition[]> GetExportedKnownTypes()
        {
            return Ok(_knownExportTypesRegistrar.GetRegisteredTypes());
        }

        /// <summary>
        /// Gets the list of available export providers
        /// </summary>
        /// <returns>The list of export providers</returns>
        [HttpGet]
        [Route("providers")]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public ActionResult<IExportProvider[]> GetExportProviders()
        {
            return Ok(_exportProviderFactories.Select(x => x(new EmptyProviderConfiguration(), null)).ToArray());
        }

        /// <summary>
        /// Provides generic viewable entities collection based on the request
        /// </summary>
        /// <param name="request">Data request</param>
        /// <returns>Viewable entities search result</returns>
        [HttpPost]
        [Route("data")]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public ActionResult<ViewableSearchResult> GetData([FromBody]ExportDataRequest request)
        {
            var currentUserName = _userNameResolver.GetCurrentUserName();
            request.DataQuery.UserName = currentUserName;

            var exportedTypeDefinition = _knownExportTypesResolver.ResolveExportedTypeDefinition(request.ExportTypeName);
            var pagedDataSource = exportedTypeDefinition.ExportedDataSourceFactory(request.DataQuery);

            return Ok(pagedDataSource.GetData());
        }

        /// <summary>
        /// Starts export task
        /// </summary>
        /// <param name="request">Export task description</param>
        /// <returns>Export task id</returns>
        [HttpPost]
        [Route("run")]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public ActionResult<PlatformExportPushNotification> RunExport([FromBody]ExportDataRequest request)
        {
            var currentUserName = _userNameResolver.GetCurrentUserName();
            request.DataQuery.UserName = currentUserName;
            var notification = new ExportPushNotification(currentUserName)
            {
                Title = $"{request.ExportTypeName} export task",
                Description = "starting export...."
            };
            _pushNotificationManager.Send(notification);

            var jobId = BackgroundJob.Enqueue<ExportJob>(x => x.ExportBackgroundAsync(request, notification, JobCancellationToken.Null, null));
            notification.JobId = jobId;

            return Ok(notification);
        }

        /// <summary>
        /// Attempts to cancel export task
        /// </summary>
        /// <param name="cancellationRequest">Cancellation request with task id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("task/cancel")]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public ActionResult CancelExport([FromBody]ExportCancellationRequest cancellationRequest)
        {
            BackgroundJob.Delete(cancellationRequest.JobId);
            return Ok();
        }

        /// <summary>
        /// Downloads file by its name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("download/{fileName}")]
        [Authorize(ModuleConstants.Security.Permissions.Download)]
        public ActionResult DownloadExportFile([FromRoute] string fileName)
        {
            var localTmpFolder = Path.GetFullPath(Path.Combine(_platformOptions.DefaultExportFolder));
            var localPath = Path.Combine(localTmpFolder, Path.GetFileName(fileName));

            //Load source data only from local file system 
            using (var stream = System.IO.File.Open(localPath, FileMode.Open))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(localPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                return PhysicalFile(localPath, contentType);
            }
        }
    }
}

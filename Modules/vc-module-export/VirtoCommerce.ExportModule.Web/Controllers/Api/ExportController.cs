using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Web.BackgroundJobs;
using VirtoCommerce.ExportModule.Web.Model;
using VirtoCommerce.Platform.Core.ExportImport.PushNotifications;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExportModule.Web.Controllers
{
    [Route("api/export")]
    public class ExportController : Controller
    {
        private readonly IEnumerable<IExportProvider> _exportProviders;
        private readonly IKnownExportTypesRegistrar _knownExportTypesRegistrar;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _pushNotificationManager;

        public ExportController(
            IEnumerable<IExportProvider> exportProviders,
            IKnownExportTypesRegistrar knownExportTypesRegistrar,
            IUserNameResolver userNameResolver,
            IPushNotificationManager pushNotificationManager)
        {
            _exportProviders = exportProviders;
            _knownExportTypesRegistrar = knownExportTypesRegistrar;
            _userNameResolver = userNameResolver;
            _pushNotificationManager = pushNotificationManager;
        }

        /// <summary>
        /// Gets the list of types ready to be exported
        /// </summary>
        /// <returns>The list of exported known types</returns>
        [HttpGet]
        [Route("knowntypes")]
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
        public ActionResult<IExportProvider[]> GetExportProviders()
        {
            return Ok(_exportProviders.ToArray());
        }

        /// <summary>
        /// Starts export task
        /// </summary>
        /// <param name="request">Export task description</param>
        /// <returns>Export task id</returns>
        [HttpPost]
        [Route("run")]
        public ActionResult<PlatformExportPushNotification> RunExport([FromBody]ExportDataRequest request)
        {
            var notification = new PlatformExportPushNotification(_userNameResolver.GetCurrentUserName())
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
        public ActionResult CancelExport([FromBody]ExportCancellationRequest cancellationRequest)
        {
            BackgroundJob.Delete(cancellationRequest.JobId);
            return Ok();
        }
    }
}

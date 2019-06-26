using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.PushNotifications;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Web.BackgroundJobs;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;

using Permission = VirtoCommerce.ImageToolsModule.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.ImageToolsModule.Web.Controllers.Api
{
    [Route("api/image/thumbnails/tasks")]
    public class ThumbnailsTasksController : Controller
    {
        private readonly IThumbnailTaskSearchService _thumbnailTaskSearchService;
        private readonly IThumbnailTaskService _thumbnailTaskService;

        private readonly IPushNotificationManager _pushNotifier;
        private readonly IUserNameResolver _userNameResolver;

        public ThumbnailsTasksController(IThumbnailTaskSearchService thumbnailTaskSearchService, IThumbnailTaskService thumbnailTaskService, IPushNotificationManager pushNotifier, IUserNameResolver userNameResolver)
        {
            _thumbnailTaskSearchService = thumbnailTaskSearchService;
            _thumbnailTaskService = thumbnailTaskService;
            _pushNotifier = pushNotifier;
            _userNameResolver = userNameResolver;
        }

        /// <summary>
        /// Creates thumbnail task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(Permission.Create)]
        public async Task<ActionResult<ThumbnailTask>> CreateThumbnailTask([FromBody]ThumbnailTask task)
        {
            await _thumbnailTaskService.SaveChangesAsync(new[] { task });
            return Ok(task);
        }

        /// <summary>
        /// Remove thumbnail tasks by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(Permission.Delete)]
        public async Task<ActionResult> DeleteThumbnailTask([FromQuery] string[] ids)
        {
            await _thumbnailTaskService.RemoveByIdsAsync(ids);
            return Ok();
        }

        /// <summary>
        /// Returns thumbnail task by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Permission.Read)]
        public async Task<ActionResult<ThumbnailTask>> GetThumbnailTask([FromRoute]string id)
        {
            var task = await _thumbnailTaskService.GetByIdsAsync(new[] { id });
            return Ok(task.FirstOrDefault());
        }

        /// <summary>
        /// Searches thumbnail options by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [Authorize(Permission.Read)]
        public async Task<ActionResult<ThumbnailTaskSearchResult>> SearchThumbnailTask([FromBody]ThumbnailTaskSearchCriteria criteria)
        {
            var result = await _thumbnailTaskSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Updates thumbnail tasks
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [Authorize(Permission.Update)]
        public async Task<ActionResult> UpdateThumbnailTask([FromBody]ThumbnailTask tasks)
        {
            await _thumbnailTaskService.SaveChangesAsync(new[] { tasks });
            return Ok();
        }

        [HttpPost]
        [Route("{jobId}/cancel")]
        [Authorize(Permission.Read)]
        public ActionResult Cancel([FromRoute]string jobId)
        {
            BackgroundJob.Delete(jobId);
            return Ok();
        }

        [HttpPost]
        [Route("run")]
        [Authorize(Permission.Read)]
        public ActionResult<ThumbnailProcessNotification> Run([FromBody]ThumbnailsTaskRunRequest runRequest)
        {
            var notification = Enqueue(runRequest);
            _pushNotifier.Send(notification);
            return Ok(notification);
        }

        private ThumbnailProcessNotification Enqueue(ThumbnailsTaskRunRequest runRequest)
        {
            var notification = new ThumbnailProcessNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Process images",
                Description = "starting process...."
            };
            _pushNotifier.Send(notification);

            var jobId = BackgroundJob.Enqueue<ThumbnailProcessJob>(x => x.Process(runRequest, notification, JobCancellationToken.Null, null));
            notification.JobId = jobId;

            return notification;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Web.BackgroundJobs
{
    public class IndexProgressHandler
    {
        private readonly ILogger _log;
        private readonly IPushNotificationManager _pushNotificationManager;

        private IDictionary<string, long> _totalCountMap;
        private IDictionary<string, long> _processedCountMap;
        private IndexProgressPushNotification _notification;
        private bool _suppressInsignificantNotifications;
        private bool _isCanceled;

        public IndexProgressHandler(ILogger<IndexProgressHandler> log, IPushNotificationManager pushNotificationManager)
        {
            _log = log;
            _pushNotificationManager = pushNotificationManager;
        }

        public void Start(string currentUserName, string notificationId, bool suppressInsignificantNotifications)
        {
            _notification = GetNotification(currentUserName, notificationId);
            _suppressInsignificantNotifications = suppressInsignificantNotifications;
            _isCanceled = false;
            _totalCountMap = new Dictionary<string, long>();
            _processedCountMap = new Dictionary<string, long>();
        }

        public void AlreadyInProgress()
        {
            _notification.ErrorCount++;
            _notification.Errors.Add("Indexation is already in progress.");
            Finish();
        }

        public void Cancel()
        {
            _isCanceled = true;
        }

        public void Progress(IndexingProgress progress)
        {
            _log.LogTrace(progress.Description);

            _totalCountMap[progress.DocumentType] = progress.TotalCount ?? 0;
            _processedCountMap[progress.DocumentType] = progress.ProcessedCount ?? 0;

            _notification.DocumentType = progress.DocumentType;
            _notification.Description = progress.Description;

            if (!progress.Errors.IsNullOrEmpty())
            {
                _notification.Errors.AddRange(progress.Errors);
                _notification.ErrorCount += _notification.Errors.Count;
            }

            _notification.TotalCount = progress.TotalCount ?? 0;
            _notification.ProcessedCount = progress.ProcessedCount ?? 0;

            if (!_suppressInsignificantNotifications || progress.TotalCount > 0 || progress.ProcessedCount > 0)
            {
                _pushNotificationManager.Send(_notification);
            }
        }

        public void Exception(Exception ex)
        {
            _notification.Errors.Add(ex.ToString());
            _notification.ErrorCount++;
        }

        public void Finish()
        {
            _notification.Finished = DateTime.UtcNow;
            _notification.TotalCount = _totalCountMap.Values.Sum();
            _notification.ProcessedCount = _processedCountMap.Values.Sum();

            _notification.Description = _isCanceled
                ? "Indexation has been canceled"
                : _suppressInsignificantNotifications
                    ? $"{_notification.DocumentType}: Indexation completed. Total: {_notification.TotalCount}, Processed: {_notification.ProcessedCount}, Errors: {_notification.ErrorCount}."
                    : "Indexation completed" + (_notification.Errors?.Any() == true ? " with errors" : " successfully");

            _log.LogTrace(_notification.Description);

            if (!_suppressInsignificantNotifications || _isCanceled || _notification.TotalCount > 0 || _notification.ProcessedCount > 0)
            {
                _pushNotificationManager.Send(_notification);
            }
        }

        public static IndexProgressPushNotification CreateNotification(string currentUserName, string notificationId)
        {
            var notification = new IndexProgressPushNotification(currentUserName ?? nameof(IndexingJobs))
            {
                Title = "Indexation process",
                Description = "Starting indexation...",
            };

            if (!string.IsNullOrEmpty(notificationId))
            {
                notification.Id = notificationId;
            }

            return notification;
        }


        private IndexProgressPushNotification GetNotification(string currentUserName, string notificationId)
        {
            IndexProgressPushNotification notification = null;

            if (!string.IsNullOrEmpty(notificationId))
            {
                var searchCriteria = new PushNotificationSearchCriteria
                {
                    Ids = new[] { notificationId }
                };

                var searchResult = _pushNotificationManager.SearchNotifies(currentUserName, searchCriteria);

                notification = searchResult?.NotifyEvents.OfType<IndexProgressPushNotification>().FirstOrDefault();
            }

            var result = notification ?? CreateNotification(currentUserName, notificationId);
            return result;
        }
    }
}

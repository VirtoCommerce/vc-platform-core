using System;
using System.Threading;
using Hangfire;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Web.BackgroundJobs
{
    /// <summary>
    /// Scales out indexation work through Hangfire.
    /// Throttles queueing so that indexation job doesn't go way faster than actual indexation work.
    /// </summary>
    public class HangfireIndexingWorker : IIndexingWorker
    {
        public const string NearRealTimeQueueName = "NearRealTimeIndexing";
        public const string BackgroundQueueName = "BackgroundIndexing";

        public int ThrottleQueueCount { get; set; } = 10;
        public int SleepTimeMs { get; set; } = 100;

        public void IndexDocuments(string documentType, string[] documentIds,
            IndexingPriority priority = IndexingPriority.Default)
        {
            if (priority != IndexingPriority.NearRealTime)
                ThrottleByQueueCount(priority, ThrottleQueueCount);

            IndexingJobs.EnqueueIndexDocuments(documentType, documentIds, IndexingPriorityToJobPriority(priority));
        }

        public void DeleteDocuments(string documentType, string[] documentIds,
            IndexingPriority priority = IndexingPriority.Default)
        {
            if (priority != IndexingPriority.NearRealTime)
                ThrottleByQueueCount(priority, ThrottleQueueCount);

            IndexingJobs.EnqueueDeleteDocuments(documentType, documentIds, IndexingPriorityToJobPriority(priority));
        }

        protected virtual void ThrottleByQueueCount(IndexingPriority priority, int maxQueueCount)
        {
            var queue = IndexingPriorityToJobPriority(priority);
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            long queued = 0;
            while (true)
            {
                queued = monitoringApi.EnqueuedCount(queue);
                if (queued <= maxQueueCount)
                    return;

                Thread.Sleep(SleepTimeMs);
            } 
        }

        protected virtual string IndexingPriorityToJobPriority(IndexingPriority priority)
        {
            switch (priority)
            {
                case IndexingPriority.NearRealTime:
                    return JobPriority.High;

                case IndexingPriority.Background:
                    return JobPriority.Low;

                default:
                    throw new ArgumentException($"Unkown priority: {priority}");
            }
        }
    }
}

using System;
using Hangfire;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SearchModule.Web.BackgroundJobs
{
    /// <summary>
    /// Implements cancellation abstraction for the job.
    /// </summary>
    /// <remarks>
    /// This is needed to support job deletion. See remarks on ICancellationToken.
    /// </remarks>
    public class JobCancellationTokenWrapper : ICancellationToken
    {
        public IJobCancellationToken JobCancellationToken { get; }

        public JobCancellationTokenWrapper(IJobCancellationToken jobCancellationToken)
        {
            JobCancellationToken = jobCancellationToken ?? throw new ArgumentNullException(nameof(jobCancellationToken));
        }

        public virtual void ThrowIfCancellationRequested()
        {
            JobCancellationToken.ThrowIfCancellationRequested();
        }
    }
}

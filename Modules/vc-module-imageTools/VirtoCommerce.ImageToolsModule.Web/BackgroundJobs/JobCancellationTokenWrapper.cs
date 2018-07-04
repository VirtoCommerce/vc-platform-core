using System;
using System.Collections.Generic;
using System.Text;
using Hangfire;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Web.BackgroundJobs
{
    /// <summary>
    /// Wrapper for Hangfire's IJobCancellationToken
    /// </summary>
    public class JobCancellationTokenWrapper : ICancellationToken
    {
        public IJobCancellationToken JobCancellationToken { get; }

        public JobCancellationTokenWrapper(IJobCancellationToken jobCancellationToken)
        {
            JobCancellationToken = jobCancellationToken;
        }

        #region Implementation of ICancellationToken

        public void ThrowIfCancellationRequested()
        {
            JobCancellationToken.ThrowIfCancellationRequested();
        }

        #endregion
    }
}

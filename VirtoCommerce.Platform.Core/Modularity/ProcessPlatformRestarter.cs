using System.Diagnostics;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public sealed class ProcessPlatformRestarter : IPlatformRestarter
    {
        public void Restart()
        {
            ProcessExtensions.StartPlatformProcess();

            Process.GetCurrentProcess()
                .KillTree();
        }
    }
}

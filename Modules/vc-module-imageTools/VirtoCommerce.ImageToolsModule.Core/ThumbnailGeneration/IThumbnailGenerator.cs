using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public interface IThumbnailGenerator
    {
        Task<ThumbnailGenerationResult> GenerateThumbnailsAsync(string sourse, string destination, IList<ThumbnailOption> options, ICancellationToken token);
    }
}

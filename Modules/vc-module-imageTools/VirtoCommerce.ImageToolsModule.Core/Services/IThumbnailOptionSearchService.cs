using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailOptionSearchService
    {
        Task<ThumbnailOptionSearchResult> SearchAsync(ThumbnailOptionSearchCriteria criteria);
    }
}

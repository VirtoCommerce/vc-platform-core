using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailOptionSearchService
    {
        ThumbnailOptionSearchResult Search(ThumbnailOptionSearchCriteria criteria);
    }
}

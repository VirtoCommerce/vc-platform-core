using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailTaskService
    {
        Task SaveOrUpdateAsync(ThumbnailTask[] options);
        Task<ThumbnailTask[]> GetByIdsAsync(string[] ids);
        Task RemoveByIdsAsync(string[] ids);
    }
}

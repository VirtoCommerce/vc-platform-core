using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailOptionService
    {
        Task SaveOrUpdateAsync(ThumbnailOption[] options);

        Task<ThumbnailOption[]> GetByIdsAsync(string[] ids);

        Task RemoveByIdsAsync(string[] ids);
    }
}

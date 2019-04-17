using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.Settings
{
    public class SettingsSearchService : ISettingsSearchService
    {
        private readonly ISettingsManager _settingsManager;

        public SettingsSearchService(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public async Task<GenericSearchResult<ObjectSettingEntry>> SearchSettingsAsync(SettingsSearchCriteria criteria)
        {
            var result = new GenericSearchResult<ObjectSettingEntry>();

            var sortInfos = GetSearchSortInfos(criteria);
            var query = GetSearchQuery(criteria, sortInfos);

            result.TotalCount = query.Count();
            var names = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Name).ToList();

            var settings = await _settingsManager.GetObjectSettingsAsync(names.ToArray());
            result.Results = settings.OrderBy(x => names.IndexOf(x.Name))
                                     .ToList();
            return result;
        }

        protected virtual IList<SortInfo> GetSearchSortInfos(SettingsSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = "Name" }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<SettingDescriptor> GetSearchQuery(SettingsSearchCriteria criteria, IList<SortInfo> sortInfos)
        {
            var query = _settingsManager.AllRegisteredSettings.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.ModuleId))
            {
                query = query.Where(x => x.ModuleId == criteria.ModuleId);
            }

            query = query.OrderBySortInfos(sortInfos);

            return query;
        }
    }
}

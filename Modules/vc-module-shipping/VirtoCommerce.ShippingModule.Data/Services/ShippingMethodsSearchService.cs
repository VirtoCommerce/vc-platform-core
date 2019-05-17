using System;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsSearchService : IShippingMethodsSearchService
    {
        private readonly Func<IShippingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memCache;
        private readonly IShippingMethodsService _shippingMethodsService;
        private readonly ISettingsManager _settingsManager;

        public ShippingMethodsSearchService(
            Func<IShippingRepository> repositoryFactory,
            IPlatformMemoryCache memCache,
            IShippingMethodsService shippingMethodsService,
            ISettingsManager settingsManager)
        {
            _repositoryFactory = repositoryFactory;
            _memCache = memCache;
            _shippingMethodsService = shippingMethodsService;
            _settingsManager = settingsManager;
        }

        public Task<ShippingMethodsSearchResult> SearchShippingMethodsAsync(ShippingMethodsSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}

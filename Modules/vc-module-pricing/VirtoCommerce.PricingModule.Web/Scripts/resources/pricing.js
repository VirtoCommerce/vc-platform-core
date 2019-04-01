angular.module('virtoCommerce.pricingModule')
    .factory('virtoCommerce.pricingModule.prices', ['$resource', function ($resource) {
        return $resource('api/products/:id/prices', { id: '@Id', catalogId: '@catalogId' }, {
            search: { url: 'api/catalog/products/prices/search' },
            getProductPrices: { isArray: true }, // is also used in other modules
            getProductPricesForWidget: { url: 'api/products/:id/:catalogId/pricesWidget', isArray: true },
            getProductPricelists: { url: 'api/catalog/products/:id/pricelists', isArray: true },
            update: { method: 'PUT' },
            remove: { method: 'DELETE', url: 'api/pricing/pricelists/:priceListId/products/prices' },
            removePrice: { method: 'DELETE', url: 'api/pricing/products/prices' }
        });
    }])
    .factory('virtoCommerce.pricingModule.pricelists', ['$resource', function ($resource) {
    	return $resource('api/pricing/pricelists/:id', {}, {
    		search: { url: 'api/pricing/pricelists' },
            update: { method: 'PUT' }
        });
    }])
    .factory('virtoCommerce.pricingModule.pricelistAssignments', ['$resource', function ($resource) {
    	return $resource('api/pricing/assignments/:id', { id: '@Id' }, {
    		search: { url: 'api/pricing/assignments' },
            getNew: { url: 'api/pricing/assignments/new' },
            update: { method: 'PUT' }
        });
    }]);
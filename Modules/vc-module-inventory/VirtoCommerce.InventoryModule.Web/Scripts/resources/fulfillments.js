angular.module('virtoCommerce.inventoryModule')
    .factory('virtoCommerce.inventoryModule.fulfillments', ['$resource', function ($resource) {
        return $resource('api/inventory/fulfillmentcenters', {}, {
            get: { url: 'api/inventory/fulfillmentcenters/:id' },
            update: { method: 'PUT' },
            search: { url: 'api/inventory/fulfillmentcenters/search', method: 'POST' }
        });
    }]);
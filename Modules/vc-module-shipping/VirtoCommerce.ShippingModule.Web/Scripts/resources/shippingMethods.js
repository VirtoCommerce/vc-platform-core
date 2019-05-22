angular.module('virtoCommerce.shippingModule')
    .factory('virtoCommerce.shippingModule.shippingMethods', ['$resource', function ($resource) {
        return $resource('api/shipping', {}, {
            search: { method: 'POST', url: 'api/shipping/search' },
            get: { url: 'api/shipping/:id' },
            update: { method: 'PUT' },
        });
    }]);

angular.module('virtoCommerce.taxModule')
.factory('virtoCommerce.taxModule.taxProviders', ['$resource', function ($resource) {
    return $resource('api/taxes', {}, {
        search: { method: 'POST', url: 'api/taxes/search' },
        get: { url: 'api/taxes/:id' },
        update: { method: 'PUT' },
    });
}]);

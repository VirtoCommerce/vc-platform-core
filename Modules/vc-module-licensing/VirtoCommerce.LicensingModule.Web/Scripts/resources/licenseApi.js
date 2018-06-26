angular.module('virtoCommerce.licensingModule')
    .factory('virtoCommerce.licensingModule.licenseApi', ['$resource', function ($resource) {
        return $resource('api/licenses/:id', null, {
            search: { method: 'POST', url: 'api/licenses/search' },
            update: { method: 'PUT' }
        });
    }]);
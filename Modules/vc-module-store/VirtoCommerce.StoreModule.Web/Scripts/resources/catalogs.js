angular.module('virtoCommerce.storeModule')
.factory('virtoCommerce.storeModule.catalogs', ['$resource', function ($resource) {
    return $resource('api/catalog/catalogs/:id', { id: '@Id' }, {      
        getCatalogs: { method: 'GET', isArray: true }       
    });
}]);

angular.module('virtoCommerce.orderModule')
.factory('virtoCommerce.orderModule.catalogItems', ['$resource', function ($resource) {
    return $resource('api/catalog/products/:id', null, {    
    });
}]);

angular.module('virtoCommerce.catalogModule')
.factory('virtoCommerce.catalogModule.aggregationProperties', ['$resource', function ($resource) {
    return $resource('', {}, {
        getProperties: { url: 'api/catalog/aggregationproperties/:storeId/properties', isArray: true },
        setProperties: { url: 'api/catalog/aggregationproperties/:storeId/properties', method: 'PUT' },
        getValues: { url: 'api/catalog/aggregationproperties/:storeId/properties/:propertyName/values', isArray: true }
    });
}]);

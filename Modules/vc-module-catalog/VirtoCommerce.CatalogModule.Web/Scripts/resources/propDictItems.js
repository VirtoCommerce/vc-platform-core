angular.module('virtoCommerce.catalogModule')
.factory('virtoCommerce.catalogModule.propDictItems', ['$resource', function ($resource) {
    return $resource('api/catalog/dictionaryitems', {}, {		
        save: { method: 'POST', url: 'api/catalog/dictionaryitems' },
        search: { method: 'POST', url: 'api/catalog/dictionaryitems/search' },
        remove: { method: 'DELETE', url: 'api/catalog/dictionaryitems' }
    });

}]);

angular.module('virtoCommerce.coreModule.currency').factory('virtoCommerce.coreModule.currency.currencyApi', ['$resource', function ($resource) {
    return $resource('api/currencies', null, {
        update: { method: 'PUT' }
    });
}]);

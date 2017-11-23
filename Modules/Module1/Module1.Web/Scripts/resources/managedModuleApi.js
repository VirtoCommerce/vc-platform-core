angular.module('virtoCommerce.samples.managed')
.factory('managedModuleApi', ['$resource', function ($resource) {
    return $resource('api/managedModule');
}]);

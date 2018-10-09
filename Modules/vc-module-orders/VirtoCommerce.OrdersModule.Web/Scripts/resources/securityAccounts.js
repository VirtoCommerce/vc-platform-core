angular.module('virtoCommerce.orderModule')
.factory('virtoCommerce.orderModule.securityAccounts', ['$resource', function($resource) {
    return $resource('api/platform/security/users/id/:id', { id: '@Id' }, {});
}]);
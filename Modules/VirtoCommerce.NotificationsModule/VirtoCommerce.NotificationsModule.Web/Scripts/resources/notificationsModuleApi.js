angular.module('virtoCommerce.notificationsModule')
    .factory('notificationsModuleApi', ['$resource', function ($resource) {
        return $resource('api/notifications');
    }]);

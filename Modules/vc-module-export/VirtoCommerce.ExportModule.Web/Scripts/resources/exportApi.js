angular.module('virtoCommerce.exportModule')
    .factory('virtoCommerce.exportModule.exportModuleApi', ['$resource', function ($resource) {
        return $resource('', null, {
            getProviders: { method: 'GET', isArray: true, url: 'api/export/providers'},
            getKnownTypes: { method: 'GET', isArray: true ,url: 'api/export/knowntypes' },
            runExport: {method: 'POST', url: 'api/export/run'},
            getData: { method: 'POST', url: 'api/export/data' },
            cancel: { method: 'POST', url: 'api/export/task/cancel' }
        });
    }]);

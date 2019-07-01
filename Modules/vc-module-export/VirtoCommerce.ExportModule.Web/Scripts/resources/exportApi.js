angular.module('virtoCommerce.exportModule')
    .factory('virtoCommerce.exportModule.exportModuleApi', ['$resource', function ($resource) {
        return $resource('', null, {
            getProviders: { method: 'GET', isArray: true, url: 'api/export/providers'},
            getKnowntypes: { method: 'GET', isArray: true ,url: 'api/export/knowntypes' },
            runExport: {method: 'POST', url: 'api/export/run'},
            cancel: {method: 'POST', url: 'api/export/task/cancel'}
        });
    }]);

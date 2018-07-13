angular.module('virtoCommerce.orderModule')
.factory('virtoCommerce.orderModule.statusTranslationService', ['$translate', function ($translate) {
    return {
        translateStatuses: function (rawStatuses, operationType) {
            return _.map(rawStatuses, function (x) {
                return {
                    key: x,
                    value: translateOrderStatus(x, operationType, $translate)
                };
            });
        }
    };
}])
// operation statuses localization filter
.filter('statusTranslate', ['$translate', function ($translate) {
    return function (statusOrOperation, operation) {
        operation = operation || statusOrOperation;
        return operation.status ? translateOrderStatus(operation.status, operation.operationType, $translate) : '';
    };
}])
;

function translateOrderStatus(rawStatus, operationType, $translate) {
    var translateKey = 'orders.settings.' + operationType.toLowerCase() + '-status.' + rawStatus.toLowerCase();
    var result = $translate.instant(translateKey);
    return result === translateKey ? rawStatus : result;
}
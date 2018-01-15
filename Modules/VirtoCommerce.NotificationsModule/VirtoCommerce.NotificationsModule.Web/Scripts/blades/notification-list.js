angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationsController', ['$scope', 'notificationsModuleApi', function ($scope, managedModuleApi) {
        var blade = $scope.blade;
        blade.title = 'Notifications';

        blade.refresh = function () {
            managedModuleApi.get(function (data) {
                blade.data = data.result;
                blade.isLoading = false;
            });
        }

        blade.refresh();
    }]);

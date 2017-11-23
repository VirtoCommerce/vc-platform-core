angular.module('virtoCommerce.samples.managed')
.controller('virtoCommerce.samples.managed.blade1Controller', ['$scope', 'managedModuleApi', function ($scope, managedModuleApi) {
    var blade = $scope.blade;
    blade.title = 'Managed API sample';

    blade.refresh = function () {
        managedModuleApi.get(function (data) {
            blade.data = data.result;
            blade.isLoading = false;
        });
    }

    blade.refresh();
}]);

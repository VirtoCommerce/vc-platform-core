angular.module('virtoCommerce.subscriptionModule')
.controller('virtoCommerce.subscriptionModule.scheduleWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.subscriptionModule.scheduleAPI', function ($scope, bladeNavigationService, scheduleAPI) {
    var blade = $scope.blade;
    $scope.loading = true;

    var isApiSave = blade.id !== 'subscriptionDetail';

    if (isApiSave) {
        scheduleAPI.get({ id: blade.itemId }, function (data) {
            $scope.schedule = data;
            $scope.loading = false;
        });
    } else {
        $scope.$watch('blade.currentEntity', function (schedule) {
            if (schedule) {
                $scope.schedule = schedule;
                $scope.loading = false;
            }
        });
    }

    $scope.openBlade = function () {
        var newBlade = {
            id: 'orderOperationChild',
            isApiSave: isApiSave,
            itemId: blade.itemId,
            data: $scope.schedule,
            controller: 'virtoCommerce.subscriptionModule.scheduleDetailController',
            template: 'Modules/$(VirtoCommerce.Subscription)/Scripts/blades/schedule-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

}]);

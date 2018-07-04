angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.fulfillmentWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.coreModule.fulfillment.fulfillments', function ($scope, bladeNavigationService, fulfillments) {
    var blade = $scope.widget.blade;

    $scope.widget.refresh = function () {
        $scope.currentNumberInfo = '...';
        fulfillments.query({}, function (results) {
            $scope.currentNumberInfo = results.length;
        }, function (error) {
            //bladeNavigationService.setError('Error ' + error.status, $scope.blade);
        });
    }

    $scope.openBlade = function () {
        var newBlade = {
            id: 'fulfillmentCenterList',
            controller: 'virtoCommerce.inventoryModule.fulfillmentListController',
            template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/fulfillment/blades/fulfillment-center-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.widget.refresh();
}]);
angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.storeFulfillmentWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            entity: blade.currentEntity,
            title: blade.title,
            subtitle: 'Fulfillment Centers of the Store',
            controller: 'virtoCommerce.inventoryModule.storeFulfillmentController',
            template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/blades/store-fulfillment-centers.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);
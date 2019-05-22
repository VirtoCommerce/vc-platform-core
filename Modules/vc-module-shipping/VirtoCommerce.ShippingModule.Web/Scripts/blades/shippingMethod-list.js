angular.module('virtoCommerce.shippingModule').controller('virtoCommerce.shippingModule.shippingMethodListController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.shippingModule.shippingMethods', function ($scope, bladeNavigationService, shippingMethods) {
    var blade = $scope.blade;

    function initializeBlade() {
        blade.isLoading = false;
        blade.headIcon = 'fa-archive';

        $scope.sortableOptions = {
            stop: function (e, ui) {
                for (var i = 0; i < $scope.blade.currentEntities.length; i++) {
                    $scope.blade.currentEntities[i].priority = i + 1;
                }
            },
            axis: 'y',
            cursor: "move"
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () { return true; }
            }
        ];

        blade.refresh();
    };

    blade.refresh = function () {
        blade.isLoading = true;
        shippingMethods.search({
            storeId: blade.storeId
        }, function (data) {
            blade.isLoading = false;
            blade.currentEntities = data.results;
            blade.selectedShippingMethods = _.findWhere(blade.currentEntities, { isActive: true });
        }, function (error) {
            bladeNavigationService.setError('Error ' + error.status, blade);
        });
    }

    $scope.selectNode = function (node) {
        $scope.selectedNodeId = node.typeName;
        var newBlade = {
            id: 'shippingMethodDetail',
            shippingMethod: node,
            storeId: blade.storeId,
            subtitle: 'shipping.blades.shipping-method-detail.subtitle',
            controller: 'virtoCommerce.shippingModule.shippingMethodDetailController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/blades/shippingMethod-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    initializeBlade();

}]);

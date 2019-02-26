angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemDimensionWidgetController', ['$scope', 'virtoCommerce.catalogModule.items', 'platformWebApp.bladeNavigationService', function ($scope, items, bladeNavigationService) {

    $scope.openBlade = function () {
        var blade = {
            id: "itemDimension",
            item: $scope.blade.item,         
            controller: 'virtoCommerce.catalogModule.itemDimensionController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-dimensions.tpl.html'
        };
        bladeNavigationService.showBlade(blade, $scope.blade);
    };
  
}]);

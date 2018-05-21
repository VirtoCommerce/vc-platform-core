angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemVariationWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openVariationListBlade = function () {
        var newBlade = {
            id: "itemVariationList",
            item: blade.item,
            catalog: blade.catalog,
            toolbarCommandsAndEvents: blade.variationsToolbarCommandsAndEvents,
            controller: 'virtoCommerce.catalogModule.itemVariationListController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-variation-list.tpl.html',
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

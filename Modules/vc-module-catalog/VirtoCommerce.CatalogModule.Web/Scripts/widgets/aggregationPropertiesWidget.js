angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.aggregationPropertiesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeFilteringProperties",
            storeId: blade.currentEntity.id,
            title: 'Filtering properties',
            controller: 'virtoCommerce.catalogModule.aggregationPropertiesController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/aggregation-properties-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

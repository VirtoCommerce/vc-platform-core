angular.module('virtoCommerce.taxModule')
.controller('virtoCommerce.taxModule.storeTaxingWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;
    
    $scope.openBlade = function () {
        var newBlade = {
            id: "taxProviders",
            title: blade.title,
            storeId: blade.currentEntityId,
            subtitle: 'stores.widgets.storeTaxingWidget.blade-subtitle',
            controller: 'virtoCommerce.taxModule.taxProviderListController',
            template: 'Modules/$(VirtoCommerce.Tax)/Scripts/blades/taxProvider-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

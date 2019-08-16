angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.productSelectorController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.selectedCount = 0;
    
    $scope.selectProducts = function () {
        var selection = blade.currentEntity.productIds || [];
        var options = {
            allowCheckingCategory: false,
            selectedItemIds: selection,
            checkItemFn: function (listItem, isSelected) {
                if (isSelected) {
                    if (!_.find(selection, function (x) { return x.id == listItem.id; })) {
                        selection.push(listItem.id);
                    }
                }
                else {
                    selection = _.reject(selection, function (x) { return x.id == listItem.id; });
                }
            }
        };
        var newBlade = {
            id: "CatalogItemsSelect",
            controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
            title: 'pricing.selectors.blades.titles.select-products',
            options: options,
            breadcrumbs: [],
            toolbarCommands: [
              {
                  name: "platform.commands.confirm", icon: 'fa fa-check',
                  executeMethod: function (pickingBlade) {
                    blade.currentEntity.productIds = _.union(blade.currentEntity.productIds, selection);
                    $scope.selectedCount = blade.currentEntity.productIds.length;
                    bladeNavigationService.closeBlade(pickingBlade);
                  },
                  canExecuteMethod: function () {
                    return _.any(selection);
                  }
              }]
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }
}]);

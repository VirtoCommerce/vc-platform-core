angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.pricesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.pricingModule.prices', function ($scope, bladeNavigationService, prices) {
    var blade = $scope.widget.blade;

    function refresh() {
        $scope.priceCount = '...';

        prices.search({
            priceListId: blade.currentEntityId,
            take: 0
        }, function (data) {
            $scope.priceCount = data.totalCount;
        });
    }

    $scope.openBlade = function () {
        var newBlade = {
            id: "pricelistChild",
            currency: blade.currentEntity.currency,
            currentEntity: blade.currentEntity,
            currentEntityId: blade.currentEntityId,
            parentWidgetRefresh: refresh,
            title: blade.title,
            subtitle: 'pricing.blades.pricelist-item-list.subtitle',
            controller: 'virtoCommerce.pricingModule.pricelistItemListController',
            template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/pricelist-item-list.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, blade);
    };

    refresh();
}]);
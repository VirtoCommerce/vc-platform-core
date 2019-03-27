angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.itemPricesWidgetController', ['$scope', '$filter', 'platformWebApp.bladeNavigationService', 'virtoCommerce.pricingModule.pricelists', 'virtoCommerce.pricingModule.prices', '$state', function ($scope, $filter, bladeNavigationService, pricelists, prices, $state) {
    var blade = $scope.blade;

    function refresh() {
        $scope.loading = true;
        return prices.getProductPricesForWidget({ id: blade.itemId, catalogId: blade.catalog.id }, function (productPrices) {
            $scope.loading = false;
            if (productPrices.length) {
                productPrices = _.groupBy(productPrices, 'currency');
                productPrices = _.max(_.values(productPrices), function (x) { return x.length; });
                var allPrices = _.union(_.pluck(productPrices, 'list'), _.pluck(productPrices, 'sale'));
                var minprice = _.min(allPrices);
                var maxprice = _.max(allPrices);
                var currency = _.any(productPrices) ? productPrices[0].currency : '';
                minprice = $filter('currency')(minprice, currency, 2);
                maxprice = $filter('currency')(maxprice, currency, 2);
                $scope.priceLabel = (minprice == maxprice ? minprice : minprice + ' - ' + maxprice);
            }
            return productPrices;
        });
    }

    $scope.openBlade = function () {
        if ($scope.loading)
            return;

        var productPricelistsBlade = {
          id: "itemPrices",
          itemId: blade.itemId,
          item: blade.item,
          parentWidgetRefresh: refresh,
          title: 'pricing.blades.item-prices.title',
          titleValues: { name: blade.item.name },
          subtitle: 'pricing.blades.item-prices.subtitle',
          controller: 'virtoCommerce.pricingModule.itemPriceListController',
          template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/item/item-prices.tpl.html'
        };
        bladeNavigationService.showBlade(productPricelistsBlade, blade);
    };

    $scope.$watch("widget.blade.catalog.id", function (id) {
        if (id) refresh();
    });

}]);
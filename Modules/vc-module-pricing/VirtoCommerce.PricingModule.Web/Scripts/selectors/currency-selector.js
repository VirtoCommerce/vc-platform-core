angular.module('virtoCommerce.pricingModule')
    .controller('virtoCommerce.pricingModule.currencySelectorController',
        [
            '$scope', 'virtoCommerce.coreModule.currency.currencyUtils',
            function($scope, currencyUtils) {
                $scope.currencies = currencyUtils.getCurrencies();
            }
        ]);

angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.pricelistSelectorController', ['$scope', 'virtoCommerce.pricingModule.pricelists', function ($scope, pricelists) {
    pricelists.search({ take: 1000 }, function (result) {
    	$scope.pricelists = result.results;
    });
}]);

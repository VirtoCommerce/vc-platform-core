angular.module('virtoCommerce.pricingModule')
    .controller('virtoCommerce.pricingModule.catalogSelectorController', ['$scope', 'virtoCommerce.catalogModule.catalogs', function ($scope, catalogs) {
        catalogs.getCatalogs({ take: 1000 }, function (result) {
            $scope.catalogs = angular.copy(result);
        });
    }]);

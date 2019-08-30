angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.catalogSelectorController', ['$scope', 'virtoCommerce.catalogModule.catalogs', function ($scope, catalogs) {
        catalogs.getCatalogs({ take: 1000 }, function (result) {
            $scope.catalogs = angular.copy(result);
        });
    }]);

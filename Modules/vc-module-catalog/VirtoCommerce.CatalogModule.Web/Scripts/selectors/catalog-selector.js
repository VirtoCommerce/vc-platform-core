angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.catalogSelectorController', ['$scope', 'virtoCommerce.catalogModule.catalogs', function ($scope, catalogs) {
    catalogs.getCatalogs(function (result) {
    	$scope.catalogs = result;
    });
}]);

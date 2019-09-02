angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.exporterListController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.catalogExportService', function ($scope, bladeNavigationService, catalogExportService) {
    var blade = $scope.blade;

    $scope.selectedNodeId = null;

	function initializeBlade() {
	    $scope.registrationsList = catalogExportService.registrationsList;
		blade.isLoading = false;
	};

	$scope.openBlade = function(data) {
        var newBlade = {};
        angular.copy(data, newBlade);

        if (data.isGenericExport) {
            var exportDataRequest = {
                exportTypeName: 'VirtoCommerce.CatalogModule.Data.ExportImport.ExportableProduct',
                isTabularExportSupported: true,
                dataQuery: {
                    exportTypeName: 'ProductExportDataQuery',
                    categoryIds : _.pluck(blade.selectedCategories, 'id'),
                    objectIds: _.pluck(blade.selectedProducts, 'id'),
                    catalogIds: [blade.catalog.id],
                    isAllSelected :true
                }
            };
            data.exportDataRequest = exportDataRequest;
            data.totalItemsCount = (newBlade.selectedProducts || []).length;

        } else {
            newBlade.selectedCategories = blade.selectedCategories;
            newBlade.selectedProducts = blade.selectedProducts;
            newBlade.catalog = blade.catalog;
        }

        bladeNavigationService.showBlade(newBlade, blade.parentBlade);

    }

    $scope.blade.headIcon = 'fa-upload';

	initializeBlade();
}]);

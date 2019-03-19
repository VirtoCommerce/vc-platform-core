﻿angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.exporterListController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.catalogExportService', function ($scope, bladeNavigationService, catalogExportService) {
    var blade = $scope.blade;

	$scope.selectedNodeId = null;

	function initializeBlade() {
	    $scope.registrationsList = catalogExportService.registrationsList;
		blade.isLoading = false;
	};

	$scope.openBlade = function (data) {
		var newBlade = {};
		angular.copy(data, newBlade);
		newBlade.selectedCategories = blade.selectedCategories;
		newBlade.selectedProducts = blade.selectedProducts;
		newBlade.catalog = blade.catalog;

		bladeNavigationService.showBlade(newBlade, blade.parentBlade);
	}

	$scope.blade.headIcon = 'fa-upload';

	initializeBlade();
}]);

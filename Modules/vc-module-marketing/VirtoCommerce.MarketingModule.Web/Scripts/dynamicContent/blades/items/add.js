angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.addContentItemsElementController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.categories', 'virtoCommerce.catalogModule.items', 'platformWebApp.dynamicProperties.api', function ($scope, bladeNavigationService, categories, items, dynamicProperties) {
	var blade = $scope.blade;
	$scope.pageSettings = {};
    $scope.pageSettings.totalItems = 0;
    $scope.pageSettings.currentPage = 1;
    $scope.pageSettings.numPages = 5;
    $scope.pageSettings.itemsPerPageCount = 999;

	blade.addFolder = function () {
		var data = { name: '', description: '', parentFolderId: blade.chosenFolder, items: [], childrenFolders: [] };
		blade.addNewFolder(data);
	};

	blade.addContentItem = function () {
		var start = $scope.pageSettings.currentPage * $scope.pageSettings.itemsPerPageCount - $scope.pageSettings.itemsPerPageCount;
		dynamicProperties.query({ id: 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem' }, function (data) {
	        angular.forEach(data, function (item){
	        	item.values = [];
	        	item.displayNames = [];
	        });
	        var contentItem = { name: '', description: '', folderId: blade.chosenFolder, dynamicProperties: data };
	        blade.addNewContentItem(contentItem);
	    });
	};

	$scope.blade.isLoading = false;
}]);

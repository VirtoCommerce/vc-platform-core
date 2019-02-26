angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemDimensionController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'virtoCommerce.catalogModule.items', 'virtoCommerce.customerModule.members', 'virtoCommerce.coreModule.packageType.packageTypeUtils', function ($scope, bladeNavigationService, settings, items, members, packageTypeUtils) {
	var blade = $scope.blade;
	blade.title = 'catalog.blades.item-dimensions.title';
	blade.subtitle = 'catalog.blades.item-dimensions.subtitle';
	blade.isLoading = false;
	$scope.isValid = false;

	var formScope;
	$scope.setForm = function (form) {
		formScope = form;
	}

	$scope.$watch("blade.currentEntity", function () {
		$scope.isValid = formScope && formScope.$valid;
	}, true);

	blade.refresh = function (item) {
		if (item) {
			initialize(item);
		}	
	};
	function initalize(item) {
		blade.item = item;

		blade.currentEntity = {
			weightUnit : item.weightUnit,
			packageType : item.packageType,
			weight : item.weight,
			measureUnit : item.measureUnit,
			height : item.height,
			width : item.width,
			length : item.length
		};

		$scope.weightUnits = settings.getValues({ id: 'VirtoCommerce.Core.General.WeightUnits' });
		$scope.measureUnits = settings.getValues({ id: 'VirtoCommerce.Core.General.MeasureUnits' });
		$scope.packageTypeUtils = packageTypeUtils;
		$scope.packageTypes = packageTypeUtils.getPackageTypes()
	};

	$scope.saveChanges = function () {
		angular.extend(blade.item, blade.currentEntity);
		$scope.bladeClose();
	};
	
    $scope.openDictionarySettingManagement = function (setting) {
    	var newBlade = {
    		id: 'settingDetailChild',
    		isApiSave: true,
    		controller: 'platformWebApp.settingDictionaryController',
    		template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
    	};
    	switch (setting) {
    		case 'WeightUnits':
    			_.extend(newBlade, {
    				currentEntityId: 'VirtoCommerce.Core.General.WeightUnits',
    				parentRefresh: function (data) { $scope.weightUnits = data; }
    			});
    			break;
    		case 'MeasureUnits':
    			_.extend(newBlade, {
    				currentEntityId: 'VirtoCommerce.Core.General.MeasureUnits',
    				parentRefresh: function (data) { $scope.measureUnits = data; }
    			});
    			break;
    	}

    	bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.$watch('blade.item.packageType', function (packageTypeId) {
    	if (packageTypeId) {
    		var packageType = _.find($scope.packageTypes, function (x) { return x.id == packageTypeId; });
    		if (packageType) {
    			blade.item.measureUnit = packageType.measureUnit;
    			blade.item.length = packageType.length;
    			blade.item.width = packageType.width;
    			blade.item.height = packageType.height;
    		}
    	}
    });

    initalize(blade.item);
}]);

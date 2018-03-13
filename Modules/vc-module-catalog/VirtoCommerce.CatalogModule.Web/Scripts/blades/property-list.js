angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.propertyListController', ['$scope', 'virtoCommerce.catalogModule.properties', 'platformWebApp.bladeNavigationService', function ($scope, properties, bladeNavigationService) {
	var blade = $scope.blade;
	$scope.isValid = false;
	blade.refresh = function (entity) {
		if (entity) {
			initialize(entity);
		}
		else {
			blade.parentBlade.refresh();
		}
	};

	function initialize(entity) {
		blade.title = entity.name;
		blade.subtitle = 'catalog.blades.property-list.subtitle';
		blade.currentEntity = entity;

		blade.currentEntities = angular.copy(entity.properties);
	
	};



	$scope.saveChanges = function () {
		blade.currentEntity.properties = blade.currentEntities;
		$scope.bladeClose();
	};

    $scope.getPropertyDisplayName = function (prop) {
        return _.first(_.map(_.filter(prop.displayNames, function (x) { return x && x.languageCode.startsWith(blade.defaultLanguage); }), function (x) { return x.name; }));      
    };

	$scope.editProperty = function (prop) {
		if (prop.isManageable) {
			var newBlade = {
				id: 'editCategoryProperty',
				currentEntityId: prop ? prop.id : undefined,
				categoryId: blade.categoryId,
				catalogId: blade.catalogId,
				defaultLanguage: blade.defaultLanguage,
                languages: blade.languages,
				controller: 'virtoCommerce.catalogModule.propertyDetailController',
				template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-detail.tpl.html'
			};
			bladeNavigationService.showBlade(newBlade, blade);
		} else {
			editUnmanageable({
				title: 'catalog.blades.item-property-detail.title',
				origEntity: prop
			});
		}
	};

	function editUnmanageable(bladeData) {
		var newBlade = {
			id: 'editItemProperty',
			properties: blade.currentEntities,
			controller: 'virtoCommerce.catalogModule.itemPropertyDetailController',
			template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-property-detail.tpl.html'
		};
		angular.extend(newBlade, bladeData);

		bladeNavigationService.showBlade(newBlade, blade);
	}

	$scope.getPropValues = function (propId, keyword) {
		return properties.values({ propertyId: propId, keyword: keyword }).$promise.then(function (result) {
			return result;
		});
	};

	var formScope;
	$scope.setForm = function (form) {
		formScope = form;
	}

	$scope.$watch("blade.currentEntities", function () {
		$scope.isValid = formScope && formScope.$valid;
	}, true);

	blade.headIcon = 'fa-gear';

	blade.toolbarCommands = [
				  {
				  	name: "catalog.commands.add-property", icon: 'fa fa-plus',
				  	executeMethod: function () {
				  		if (blade.entityType == "product") {
				  			editUnmanageable({
				  				isNew: true,
				  				title: 'catalog.blades.item-property-detail.title-new',
				  				origEntity: {
				  					type: "Product",
				  					valueType: "ShortText",
				  					values: []
				  				}
				  			});
				  		} else {
				  			$scope.editProperty({ isManageable: true });
				  		};
				  	},
				  	canExecuteMethod: function () {
				  		return true;
				  	}
				  },

	];
	blade.isLoading = false;
	initialize(blade.currentEntity);
}]);

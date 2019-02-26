angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.catalogPropertyWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
	var blade = $scope.blade;

	$scope.openCatalogPropertyBlade = function () {
		var newBlade = {
			id: "catalogPropertyDetail",
			currentEntity: blade.currentEntity,
			languages: _.pluck(blade.currentEntity.languages, 'languageCode'),
			defaultLanguage: blade.currentEntity.defaultLanguage.languageCode,
			entityType: "catalog",
			catalogId: blade.currentEntity.id,
			propGroups: [{ title: 'catalog.properties.catalog', type: 'Catalog' }, { title: 'catalog.properties.category', type: 'Category' }, { title: 'catalog.properties.product', type: 'Product' }, { title: 'catalog.properties.variation', type: 'Variation' }],
			controller: 'virtoCommerce.catalogModule.propertyListController',
			template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-list.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, blade);
	};
}]);

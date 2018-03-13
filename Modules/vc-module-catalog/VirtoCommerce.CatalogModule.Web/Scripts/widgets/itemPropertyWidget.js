angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemPropertyWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;
    $scope.propertiesCount = '...';

    $scope.$watch('blade.item', function (product) {
        if (product)
            $scope.propertiesCount = _.filter(product.properties, function (x) { return x.type == 'Product' || x.type == 'Variation'; }).length;
    });

    $scope.openItemPropertyBlade = function () {
        var newBlade = {
        	id: "itemProperty",
        	productId: blade.currentEntity.id,
        	entityType: "product",
        	currentEntity: blade.currentEntity,
        	languages: _.pluck(blade.catalog.languages, 'languageCode'),
        	defaultLanguage: blade.catalog.defaultLanguage.languageCode,
            propGroups: [ {title:'catalog.properties.product', type:'Product'}, {title:'catalog.properties.variation', type:'Variation'} ],
            controller: 'virtoCommerce.catalogModule.propertyListController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

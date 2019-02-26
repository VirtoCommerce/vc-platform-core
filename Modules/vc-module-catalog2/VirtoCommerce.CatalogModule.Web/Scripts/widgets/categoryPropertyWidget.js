angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.categoryPropertyWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;
    $scope.propertiesCount = '...';

    $scope.$watch('blade.currentEntity', function (product) {
        if (product)
            $scope.propertiesCount = _.filter(product.properties, function (x) { return x.type == 'Category' || x.type == 'Product' || x.type == 'Variation'; }).length;
    });

    $scope.openPropertiesBlade = function () {
        var newBlade = {
            id: "categoryPropertyDetail",
            categoryId: blade.currentEntity.id,
            entityType: "category",
            currentEntity: blade.currentEntity,
            languages: _.pluck(blade.catalog.languages, 'languageCode'),
            defaultLanguage: blade.catalog.defaultLanguage.languageCode,
            propGroups: [{ title: 'catalog.properties.category', type: 'Category' }, { title: 'catalog.properties.product', type: 'Product' }, { title: 'catalog.properties.variation', type: 'Variation' }],
            controller: 'virtoCommerce.catalogModule.propertyListController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

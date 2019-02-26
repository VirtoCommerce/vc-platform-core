angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.catalogEntryImageWidgetController',
    ['$scope', 'virtoCommerce.catalogModule.items', 'virtoCommerce.catalogModule.categories', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.catalogImagesFolderPathHelper',
        function ($scope, items, categories, bladeNavigationService, catalogImgHelper) {

        $scope.openBlade = function () {
            var blade = {
                id: "itemImage",
                item: $scope.blade.currentEntity,
                folderPath: catalogImgHelper.getImagesFolderPath($scope.blade.currentEntity.catalogId, $scope.blade.currentEntity.code),
                controller: 'virtoCommerce.catalogModule.imagesController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/images.tpl.html'
            };
            bladeNavigationService.showBlade(blade, $scope.blade);
        };

        function setCurrentEntities(images) {
            if (images) {
                $scope.currentEntities = images;
            }
        }
        $scope.$watch('blade.item.images', setCurrentEntities);
        $scope.$watch('blade.currentEntity.images', setCurrentEntities);
    }]);

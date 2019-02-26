angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.editorialReviewWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    $scope.currentBlade = $scope.widget.blade;

    $scope.openBlade = function () {
        var blade = {
            id: "editorialReviewsList",
            item: $scope.currentBlade.item,
            catalog: $scope.currentBlade.catalog,
            controller: 'virtoCommerce.catalogModule.editorialReviewsListController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/editorialReviews-list.tpl.html'
        };

        bladeNavigationService.showBlade(blade, $scope.currentBlade);
    };

}]);

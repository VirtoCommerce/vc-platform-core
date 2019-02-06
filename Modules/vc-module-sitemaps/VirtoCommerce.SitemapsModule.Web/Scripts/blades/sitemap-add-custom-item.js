angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.sitemapItemsAddCustomItemController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    blade.toolbarCommands = [{
        name: 'sitemapsModule.blades.addCustomItem.toolbar.save',
        icon: 'fa fa-save',
        executeMethod: function () {
            blade.confirmChangesFn([{
                title: blade.currentEntity.urlTemplate,
                objectType: 'Custom',
                urlTemplate: blade.currentEntity.urlTemplate
            }], blade);
        },
        canExecuteMethod: function () {
            return $scope.formScope && $scope.formScope.$valid;
        }
    }];

    $scope.setForm = function (form) { $scope.formScope = form; };

    blade.currentEntity = {};
    blade.isLoading = false;
}]);
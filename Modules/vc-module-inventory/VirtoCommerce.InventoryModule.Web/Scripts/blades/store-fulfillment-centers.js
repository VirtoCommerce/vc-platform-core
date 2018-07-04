angular.module('virtoCommerce.inventoryModule')
.controller('virtoCommerce.inventoryModule.storeFulfillmentController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.inventoryModule.fulfillments', function ($scope, bladeNavigationService, fulfillments) {
    $scope.saveChanges = function () {
        angular.copy($scope.blade.currentEntity, $scope.blade.origEntity);
        $scope.bladeClose();
    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    }

    $scope.isValid = function () {
        return $scope.formScope && $scope.formScope.$valid;
    }

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    }

    $scope.blade.refresh = function () {
        getFulfillmentCenters();
    }

    $scope.openFulfillmentCentersList = function () {
        var newBlade = {
            id: 'fulfillmentCenterList',
            controller: 'virtoCommerce.inventoryModule.fulfillmentListController',
            template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/blades/fulfillment-center-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    }

    $scope.blade.headIcon = 'fa-archive';

    getFulfillmentCenters();

    $scope.blade.isLoading = false;
    $scope.blade.currentEntity = angular.copy($scope.blade.entity);
    $scope.blade.origEntity = $scope.blade.entity;

    function getFulfillmentCenters() {
        fulfillments.search({ take: 100 }, function (response) {
            $scope.fulfillmentCenters = response.results;
        });
    }
    
}]);
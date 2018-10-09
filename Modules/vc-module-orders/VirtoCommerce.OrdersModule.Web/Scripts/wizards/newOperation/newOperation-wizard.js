angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.newOperationWizardController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.orderModule.knownOperations', function ($scope, bladeNavigationService, knownOperations) {
    var blade = $scope.blade;

    $scope.availableOperations = _.map(blade.availableTypes, function (type) {
        return knownOperations.getOperation(type);
    });

    $scope.openDetailsBlade = function (op) {
        var newBlade = angular.copy(op.detailBlade);        
        newBlade.customerOrder = blade.customerOrder;
        newBlade.isNew = true;

        bladeNavigationService.showBlade(newBlade, blade.parentBlade);
    };

    blade.isLoading = false;
}]);

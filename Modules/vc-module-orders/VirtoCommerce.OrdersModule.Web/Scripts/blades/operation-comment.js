angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.orderOperationCommentDetail', ['$scope', function ($scope) {
    var blade = $scope.blade;

    function initializeBlade() {
        blade.origEntity = blade.currentEntity;
        blade.currentEntity = angular.copy(blade.origEntity.comment);
        blade.isLoading = false;
    }

    $scope.cancelChanges = function () { $scope.bladeClose(); };

    $scope.isValid = function () { return true; };

    $scope.saveChanges = function () {
        blade.origEntity.comment = blade.currentEntity;
        $scope.bladeClose();
    };

    initializeBlade();
}]);
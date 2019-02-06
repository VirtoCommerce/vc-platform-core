angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.baseUrlDialogController', ['$scope', '$modalInstance', 'dialog', function ($scope, $modalInstance, dialog) {
    angular.extend($scope, dialog);

    $scope.confirm = function () {
        $modalInstance.close($scope.baseUrl);
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    }
}]);
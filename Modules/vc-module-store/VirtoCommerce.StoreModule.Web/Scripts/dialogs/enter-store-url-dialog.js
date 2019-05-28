angular.module('virtoCommerce.storeModule')
    .controller('virtoCommerce.storeModule.enterStoreUrlDialogController', ['$scope', '$modalInstance', 'dialog', function ($scope, $modalInstance) {
        angular.extend($scope);

        $scope.confirm = function() {
            $modalInstance.close($scope.secureUrl);
        };

        $scope.cancel = function() {
            $modalInstance.dismiss('cancel');
        };
    }]);

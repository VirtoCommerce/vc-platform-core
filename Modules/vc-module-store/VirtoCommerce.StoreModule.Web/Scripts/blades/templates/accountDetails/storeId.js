angular.module('virtoCommerce.storeModule')
.controller('virtoCommerce.storeModule.accountDetails.storeIdController', ['$scope', 'virtoCommerce.storeModule.stores', function ($scope, stores) {
    $scope.$watch("blade.currentEntity.storeId", function (storeId) {
        if (storeId) {
            stores.get({ id: storeId }, function(data) {
                $scope.storeName = data.name;
            });
        }
    });
}]);
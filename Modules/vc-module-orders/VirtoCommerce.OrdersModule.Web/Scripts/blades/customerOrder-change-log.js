angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.customerOrderChangeLogController', ['$scope', 'virtoCommerce.orderModule.order_res_customerOrders', 
function ($scope, customerOrders) {

    customerOrders.getOrderChanges({
        id: $scope.blade.orderId
    }, function (data) {
        $scope.blade.isLoading = false;
        $scope.blade.currentEntities = data;
    });

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;
    };
}]);

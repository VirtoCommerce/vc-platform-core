angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentTransactionsDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', function ($scope, bladeNavigationService, dialogService) {
    var blade = $scope.blade;
    blade.isLoading = false;
    blade.title = 'orders.blades.transaction-detail.title';
    blade.subtitle = 'orders.blades.transaction-detail.subtitle';
}]);
angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentTransactionsWidgetController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', function ($scope, $translate, bladeNavigationService) {
    var blade = $scope.widget.blade;

    $scope.$watch('widget.blade.currentEntity', function (operation) {
        $scope.operation = operation;
    });

    $scope.openItemsBlade = function () {
            var newBlade = {
                id: 'transactions',              
                currentEntity: $scope.operation,
                controller: 'virtoCommerce.orderModule.paymentTransactionsListController',
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/payment-transactions-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);        
    };
}]);

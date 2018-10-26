angular.module('virtoCommerce.subscriptionModule')
.controller('virtoCommerce.subscriptionModule.subscriptionOrdersWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        if (!blade.isLoading) {
            var newBlade = {
                id: 'subscriptionOrders',
                preloadedOrders: blade.currentEntity.customerOrders,
                title: 'subscription.blades.subscriptionOrder-list.title',
                controller: 'virtoCommerce.orderModule.customerOrderListController',
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/customerOrder-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }
    };

}]);

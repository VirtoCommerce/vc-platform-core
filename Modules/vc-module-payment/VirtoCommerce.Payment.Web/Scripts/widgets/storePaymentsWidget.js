angular.module('virtoCommerce.paymentModule').controller('virtoCommerce.paymentModule.storePaymentsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            storeId: blade.currentEntity.id,
            title: blade.title,
            subtitle: 'payment.widgets.store-payment-widget.blade-subtitle',
            controller: 'virtoCommerce.paymentModule.paymentMethodListController',
            template: 'Modules/$(VirtoCommerce.Payment)/Scripts/blades/paymentMethod-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);

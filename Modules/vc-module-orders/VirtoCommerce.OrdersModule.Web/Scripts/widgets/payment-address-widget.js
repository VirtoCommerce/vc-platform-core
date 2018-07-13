angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentAddressWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    $scope.operation = {};

    $scope.openBlade = function () {
        var address = $scope.operation.billingAddress;
        if (!address) {
            address = { isNew: true, addressType: 'billing' };
        }

        var newBlade = {
            id: 'orderOperationAddresses',
            title: 'orders.widgets.payment-address.blade-title',
            currentEntity: address,
            controller: 'virtoCommerce.coreModule.common.coreAddressDetailController',
            template: 'Modules/$(VirtoCommerce.Core)/Scripts/common/blades/address-detail.tpl.html',
            deleteFn: function (address) {
                $scope.operation.billingAddress = null
            },
            confirmChangesFn: function (address) {
                $scope.operation.billingAddress = address;
                address.isNew = false;
            }
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    $scope.getAddressName = function (address) {
        var retVal = null;
        if (address) {
            retVal = [address.countryCode, address.regionName, address.city, address.line1].join(",");
        }
        return retVal;
    };

    $scope.$watch('widget.blade.currentEntity', function (operation) {
        $scope.operation = operation;
    });
}]);

angular.module('virtoCommerce.orderModule')
    .controller('virtoCommerce.orderModule.customerOrderChangeLogWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.widget.blade;

        blade.showOrderChanges = function () {
            var newBlade = {
                id: 'customerOrderChangeLog',
                orderId: blade.customerOrder.id,
                headIcon: blade.headIcon,
                title: blade.title,
                subtitle: 'platform.widgets.operations.blade-subtitle',
                isExpandable: true,
                controller: 'virtoCommerce.orderModule.customerOrderChangeLogController',
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/customerOrder-change-log.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };
    }]);

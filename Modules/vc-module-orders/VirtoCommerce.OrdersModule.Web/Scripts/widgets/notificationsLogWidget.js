angular.module('virtoCommerce.orderModule')
    .controller('virtoCommerce.orderModule.notificationsLogWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.widget.blade;

        blade.showNotificationsLog = function () {
            var objectId = blade.currentEntity.id;
            var objectTypeId = 'CustomerOrder';
            var newBlade = {
                id: 'notificationLogWidgetChild',
                objectId: objectId,
                objectTypeId: objectTypeId,
                title: 'orders.widgets.notifications.title',
                subtitle: 'orders.widgets.notifications.subtitle',
                controller: 'platformWebApp.notificationsJournalController',
                template: '$(Platform)/Scripts/app/notifications/blades/notifications-journal.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };
    }]);
angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsMenuController', ['$scope', '$stateParams', 'platformWebApp.bladeNavigationService', function ($scope, $stateParams, bladeNavigationService) {
    var blade = $scope.blade;
    blade.updatePermission = 'notifications:read';

    function initializeBlade() {
        var entities = [
            { id: '1', name: 'notifications.blades.notifications-list.title', templateName: 'notifications-list', controllerName: 'notificationsListController', icon: 'fa-list', subtitle: 'notifications.blades.notifications-list.subtitle' },
            { id: '2', name: 'notifications.blades.notifications-journal.title', templateName: 'notifications-journal', controllerName: 'notificationsJournalController', icon: 'fa-book', subtitle: 'notifications.blades.notifications-journal.subtitle' }];
        blade.currentEntities = entities;
        blade.isLoading = false;
    };

    blade.openBlade = function (data) {
        if (!blade.hasUpdatePermission()) return;

        $scope.selectedNodeId = data.id;

        var tenantId = $stateParams.objectId;
        var tenantType = $stateParams.objectTypeId;
        var newBlade = {
            id: 'notificationsList',
            title: data.name,
            tenantId: tenantId,
            tenantType: tenantType,
            subtitle: data.subtitle,
            controller: 'virtoCommerce.notificationsModule.' + data.controllerName,
            template: 'Modules/$(VirtoCommerce.Notifications)/Scripts/blades/' + data.templateName + '.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    blade.headIcon = 'fa-envelope';

    initializeBlade();

}]);

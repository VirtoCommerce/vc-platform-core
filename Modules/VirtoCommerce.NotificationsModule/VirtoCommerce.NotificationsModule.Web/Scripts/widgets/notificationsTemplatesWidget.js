angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsTemplatesWidgetController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', function ($scope, $translate, bladeNavigationService) {
	var blade = $scope.widget.blade;
    $scope.templatesCount = '...';
    
    $scope.$watch('blade.currentEntity', function (notification) {
        if (notification && notification.templates)
            $scope.templatesCount = notification.templates.length;
    });

	blade.showTemplates = function () {
		var objectId = blade.currentEntity.id;
		var objectTypeId = 'Notifications';
		var newBlade = {
			id: 'notificationTemplatesWidgetChild',
			title: 'notifications.widgets.notificationsTemplatesWidget.blade-title',
			titleValues: { displayName: $translate.instant('notificationTypes.' + blade.currentEntity.type + '.displayName') },
			objectId: objectId,
			objectTypeId: objectTypeId,
            currentEntity : blade.currentEntity,
            subtitle: 'notifications.widgets.notificationsTemplatesWidget.blade-subtitle',
			controller: 'virtoCommerce.notificationsModule.notificationTemplatesListController',
			template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notification-templates-list.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, blade);
	};
}]);

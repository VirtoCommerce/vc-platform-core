angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsTemplatesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
	var blade = $scope.widget.blade;

	blade.showTemplates = function () {
		var objectId = blade.currentEntity.id;
		var objectTypeId = 'Notifications';
		var newBlade = {
			id: 'notificationTemplatesWidgetChild',
			title: 'notifications.widgets.notificationsTemplatesWidget.blade-title',
			titleValues: { id: blade.currentEntity.id },
			objectId: objectId,
			objectTypeId: objectTypeId,
			languages: blade.currentEntity.languages,
			subtitle: 'notifications.widgets.notificationsTemplatesWidget.blade-subtitle',
			controller: 'virtoCommerce.notificationsModule.notificationTemplatesListController',
			template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notification-templates-list.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, blade);
	};
}]);

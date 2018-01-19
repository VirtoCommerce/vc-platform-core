angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsAttachmentsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
	var blade = $scope.widget.blade;

	blade.showLog = function () {
		var objectId = blade.currentEntity.id;
        var objectTypeId = 'Notifications';
		var newBlade = {
            id: 'notificationAttachmentsWidgetChild',
            title: 'notifications.widgets.notificationsAttachmentsWidget.blade-title',
			titleValues: { id: blade.currentEntity.id },
			objectId: objectId,
			objectTypeId: objectTypeId,
			languages: blade.currentEntity.languages,
            subtitle: 'notifications.widgets.notificationsAttachmentsWidget.blade-subtitle',
            controller: 'virtoCommerce.notificationsModule.notificationsJournalController',
			template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-journal.tpl.html'
		};
		bladeNavigationService.showBlade(newBlade, blade);
	};
}]);

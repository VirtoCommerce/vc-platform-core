angular.module('virtoCommerce.subscriptionModule')
	.controller('virtoCommerce.subscriptionModule.notificationsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
		var blade = $scope.widget.blade;

		blade.showNotifications = function () {
			var objectId = blade.currentEntity.id;
			var objectTypeId = 'Subscription';
			var newBlade = {
				id: 'notificationWidgetChild',
				objectId: objectId,
				objectTypeId: objectTypeId,
				// languages: blade.currentEntity.languages,
				title: 'subscription.widgets.notificationsWidget.blade-title',
				subtitle: 'subscription.widgets.notificationsWidget.blade-subtitle',
				controller: 'platformWebApp.notificationsListController',
				template: '$(Platform)/Scripts/app/notifications/blades/notifications-list.tpl.html'
			};
			bladeNavigationService.showBlade(newBlade, blade);
		};
	}]);
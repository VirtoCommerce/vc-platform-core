angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationJournalDetailsController', ['$scope', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'platformWebApp.bladeNavigationService', function ($scope, notifications, bladeNavigationService) {
	var blade = $scope.blade;

	blade.initialize = function () {
		notifications.getNotificationJournalDetails({ id: blade.currentNotificationId }, function (data) {
			blade.currentEntity = data;
			blade.isLoading = false;
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, $scope.blade);
		});
	}

	blade.initialize();
}]);

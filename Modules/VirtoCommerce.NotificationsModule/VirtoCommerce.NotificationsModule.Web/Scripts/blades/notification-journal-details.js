angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationJournalDetailsController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'platformWebApp.bladeNavigationService', 'platformWebApp.notifications', function ($scope, notificationsService, bladeNavigationService, notifications) {
	var blade = $scope.blade;

	blade.initialize = function () {
//		notifications.getNotificationJournalDetails({ id: blade.currentNotificationId }, function (data) {
//			blade.currentEntity = data;
//			blade.isLoading = false;
//		}, function (error) {
//			bladeNavigationService.setError('Error ' + error.status, $scope.blade);
//		});
        notificationsService.getNotificationJournalDetails({ id: blade.currentNotificationId }).then(function(data){
            blade.currentEntity = data;
            blade.isLoading = false;
        }, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	}

	blade.initialize();
}]);

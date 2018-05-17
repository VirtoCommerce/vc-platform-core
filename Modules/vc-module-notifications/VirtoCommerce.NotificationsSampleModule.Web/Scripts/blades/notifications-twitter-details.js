angular.module('virtoCommerce.notificationsSampleModule')
.controller('virtoCommerce.notificationsSampleModule.editTwitterController', ['$scope', '$filter', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', function($scope, $filter, notifications, bladeNavigationService, dialogService) {
    var blade = $scope.blade; 
    
    blade.initialize = function () {
		blade.isLoading = true;
        notifications.getNotificationByType({ type: blade.type }, function(data) {
            blade.isLoading = false;
            setNotification(data);
        })
    };
    
    function setNotification(data) {
		blade.currentEntity = angular.copy(data);
		blade.currentEntity.tenantIdentity = { tenantId: "NotificationSampleId", tenantType: "NotificationSampleType"};
		_.map(blade.currentEntity.templates, function (template) {
			template.createdDateAsString = $filter('date')(template.createdDate, "yyyy-MM-dd"); 
			template.modifiedDateAsString = $filter('date')(template.modifiedDate, "yyyy-MM-dd"); 
			return template;
		});
		if (!blade.currentEntity.templates) blade.currentEntity.templates = [];
		blade.origEntity = angular.copy(blade.currentEntity);
        $scope.isValid = false;
    };
    
    blade.updateNotification = function () {
		blade.isLoading = true;
        notifications.updateNotification({ type: blade.type }, blade.currentEntity, function () {
			blade.isLoading = false;
			blade.origEntity = angular.copy(blade.currentEntity);
			blade.parentBlade.refresh();
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	};

	$scope.blade.toolbarCommands = [
		{
			name: "platform.commands.save", icon: 'fa fa-save',
			executeMethod: blade.updateNotification,
			canExecuteMethod: canSave,
			permission: blade.updatePermission
		},
		{
			name: "platform.commands.undo", icon: 'fa fa-undo',
			executeMethod: function () {
				blade.currentEntity = angular.copy(blade.origEntity);
			},
			canExecuteMethod: isDirty,
			permission: blade.updatePermission
		}
    ];
    
    $scope.$watch("blade.currentEntity", function () {
		$scope.isValid = $scope.formScope && $scope.formScope.$valid;
	}, true); 

	function isDirty() {
        return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
	}

	function canSave() {
        return isDirty() && $scope.isValid;
	}

	blade.onClose = function (closeCallback) {
		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, blade.updateTemplate, closeCallback, "notifications.dialogs.notification-details-save.title", "notifications.dialogs.notification-details-save.message");
    };
    
    var formScope; 
    $scope.setForm = function (form) { 
        $scope.formScope = form; 
    }

    blade.headIcon = 'fa-twitter';

    blade.initialize();
}]);
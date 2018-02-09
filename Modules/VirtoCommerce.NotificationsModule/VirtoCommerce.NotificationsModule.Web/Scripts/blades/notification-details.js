angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsEditController', ['$rootScope', '$scope', '$timeout', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService',
function ($rootScope, $scope, $timeout, notifications, bladeNavigationService, dialogService) {
	$scope.setForm = function (form) { $scope.formScope = form; }

	var blade = $scope.blade;
	blade.updatePermission = 'platform:notification:update';
	var codemirrorEditor;
	blade.parametersForTemplate = [];
    $scope.isValid = false;

	blade.initialize = function () {
		blade.isLoading = true;
        notifications.getNotificationByType({ type: blade.type }, function(data) {
            blade.isLoading = false;
            setNotification(data);
        })
	};
    
	function setNotification(data) {
		data.objectId = blade.objectId;
		data.objectTypeId = blade.objectTypeId;
		blade.origEntity = angular.copy(data);
		blade.currentEntity = data;
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
			canExecuteMethod: canSave
		},
		{
			name: "platform.commands.undo", icon: 'fa fa-undo',
			executeMethod: function () {
				blade.currentEntity = angular.copy(blade.origEntity);
			},
			canExecuteMethod: isDirty
		}
	];

	$scope.editorOptions = {
		lineWrapping: true,
		lineNumbers: true,
		parserfile: "liquid.js",
		extraKeys: { "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); } },
		foldGutter: true,
		gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
		onLoad: function (_editor) {
			codemirrorEditor = _editor;
		},
		mode: "liquid-html"
	};
    
    $scope.$watch("blade.currentEntity", function () {
		$scope.isValid = $scope.formScope && $scope.formScope.$valid;
	}, true); 

	function isDirty() {
        var eq = angular.equals(blade.origEntity, blade.currentEntity);
        return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
	}

	function canSave() {
		return isDirty() && $scope.isValid;//$scope.formScope && $scope.formScope.$valid;
	}

	blade.onClose = function (closeCallback) {
		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, blade.updateTemplate, closeCallback, "notifications.dialogs.notification-details-save.title", "notifications.dialogs.notification-details-save.message");
	};

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

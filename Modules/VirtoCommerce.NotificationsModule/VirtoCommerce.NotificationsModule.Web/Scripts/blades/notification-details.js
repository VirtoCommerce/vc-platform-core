angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsEditController', ['$rootScope', '$scope', '$timeout', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService',
function ($rootScope, $scope, $timeout, notifications, bladeNavigationService, dialogService) {
	$scope.setForm = function (form) { $scope.formScope = form; }

	var blade = $scope.blade;
	blade.updatePermission = 'platform:notification:update';
	var codemirrorEditor;
	blade.parametersForTemplate = [];

	blade.initialize = function () {
		blade.isLoading = true;
        notifications.getNotificationByType({ type: blade.notificationType }, function(data) {
            blade.isLoading = false;
            setNotification(data);
        })
	};

	function setNotification(data) {
		data.objectId = blade.objectId;
		data.objectTypeId = blade.objectTypeId;
		blade.origEntity = _.clone(data);
		blade.currentEntity = data;
	};

	blade.updateNotification = function () {
		blade.isLoading = true;
		notifications.updateNotification({ type: blade.notificationType }, blade.currentEntity, function () {
			blade.isLoading = false;
			blade.origEntity = _.clone(blade.currentEntity);
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
				blade.currentEntity = _.clone(blade.origEntity);
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
		return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
	}

	function canSave() {
		return $scope.isValid;//isDirty() && $scope.formScope && $scope.formScope.$valid;
	}

	blade.onClose = function (closeCallback) {
		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, blade.updateTemplate, closeCallback, "notifications.dialogs.notification-details-save.title", "notifications.dialogs.notification-details-save.message");
	};

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsEditController', ['$rootScope', '$scope', '$timeout', 'virtoCommerce.notificationsModule.notificationsService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.notifications',
function ($rootScope, $scope, $timeout, notificationsService, bladeNavigationService, dialogService, notifications) {
	$scope.setForm = function (form) { $scope.formScope = form; }

	var blade = $scope.blade;
	blade.updatePermission = 'platform:notification:update';
	var codemirrorEditor;
	blade.parametersForTemplate = [];

	blade.initialize = function () {
		blade.isLoading = true;
		notificationsService.getNotificationByType({ type: blade.notificationType }).then( function (data) {
			blade.isLoading = false;
			setNotification(data);
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	};

	function setNotification(data) {
		data.objectId = blade.objectId;
		data.objectTypeId = blade.objectTypeId;
		blade.origEntity = _.clone(data);
		blade.currentEntity = data;
		// $timeout(function () {
		// 	if (codemirrorEditor) {
		// 		codemirrorEditor.refresh();
		// 		codemirrorEditor.focus();
		// 	}
		// 	blade.origEntity = angular.copy(blade.currentEntity);
		// }, 1);
	};

	blade.updateTemplate = function () {
		blade.isLoading = true;
		notifications.updateTemplate({}, blade.currentEntity, function () {
			blade.isLoading = false;
			blade.origEntity = _.clone(blade.currentEntity);
			if (!blade.isNew) {
				blade.parentBlade.initialize();
			}
			else {
				blade.isNew = false;
				if (!blade.isFirst) {
					blade.parentBlade.initialize();
					bladeNavigationService.closeBlade(blade);
				}
				else {
					blade.parentBlade.openList(blade.notificationType, blade.objectId, blade.objectTypeId);
				}
			}
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	};

	blade.delete = function () {
		notifications.deleteTemplate({ id: blade.currentEntity.id }, function (data) {
			blade.parentBlade.initialize();
			bladeNavigationService.closeBlade(blade);
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	}

	$scope.blade.toolbarCommands = [
		{
			name: "platform.commands.save", icon: 'fa fa-save',
			executeMethod: blade.updateTemplate,
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

	function isDirty() {
		return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
	}

	function canSave() {
		return isDirty() && $scope.formScope && $scope.formScope.$valid;
	}

	blade.onClose = function (closeCallback) {
		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, blade.updateTemplate, closeCallback, "notifications.dialogs.notification-details-save.title", "notifications.dialogs.notification-details-save.message");
	};

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

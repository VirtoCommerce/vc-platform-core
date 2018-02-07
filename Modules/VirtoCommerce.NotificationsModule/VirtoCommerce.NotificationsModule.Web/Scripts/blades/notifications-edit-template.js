angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.editTemplateController', ['$rootScope', '$scope', '$timeout', '$localStorage', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'FileUploader', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 
 function ($rootScope, $scope, $timeout, $localStorage, notifications, FileUploader, bladeNavigationService, dialogService) {
    $scope.isValid = false;
     
    var formScope; 
    $scope.setForm = function (form) { 
        formScope = form; 
    }

    var blade = $scope.blade;
    blade.updatePermission = 'platform:notification:update';
    var codemirrorEditor;
    blade.parametersForTemplate = [];
    var keyTemplateLocalStorage;

    $scope.saveChanges = function () {
        //blade.currentEntity.properties = blade.currentEntities;
        $localStorage[keyTemplateLocalStorage] = blade.currentEntity.dynamicProperties;
        $scope.bladeClose();
    };
     
     
    //todo 
    var contentType = 'image';//blade.contentType.substr(0, 1).toUpperCase() + blade.contentType.substr(1, blade.contentType.length - 1);
    $scope.fileUploader = new FileUploader({
        url: 'api/platform/assets?folderUrl=cms-content/' + contentType + '/assets',
        headers: { Accept: 'application/json' },
        autoUpload: true,
        removeAfterUpload: true,
        onBeforeUploadItem: function (fileItem) {
            blade.isLoading = true;
        },
        onSuccessItem: function (fileItem, response, status, headers) {
            $scope.$broadcast('filesUploaded', { items: response });
        },
        onErrorItem: function (fileItem, response, status, headers) {
            bladeNavigationService.setError(fileItem._file.name + ' failed: ' + (response.message ? response.message : status), blade);
        },
        onCompleteAll: function () {
            blade.isLoading = false;
        }
    });
     
    function setTemplate(data) {
//        data.notificationType = blade.notificationType;
//        data.displayName = blade.displayName;
//		data.objectId = blade.objectId;
//		data.objectTypeId = blade.objectTypeId;
//        data.sendGatewayType = blade.sendGatewayType;
        keyTemplateLocalStorage = blade.objectTypeId + '.' + blade.notification.notificationType;
		blade.origEntity = _.clone(data);
		blade.isLoading = false;
        if (!blade.currentEntity.id) {
            blade.isNew = true;    
        }
        else {
            var itemFromLocalStorage = $localStorage[keyTemplateLocalStorage];
//            if (itemFromLocalStorage) {
//                blade.currentEntity.dynamicProperties = itemFromLocalStorage;
//            }    
        }

		$timeout(function () {
			if (codemirrorEditor) {
				codemirrorEditor.refresh();
				codemirrorEditor.focus();
			}
			blade.origEntity = angular.copy(blade.currentEntity);
		}, 1);
	};

	blade.initialize = function () {
		blade.isLoading = true;
        blade.currentEntity = angular.copy(blade.currentEntity);
        setTemplate(blade.currentEntity);
	};

	blade.delete = function () {
		notifications.deleteTemplate({ id: blade.currentEntity.id }, function (data) {
			blade.parentBlade.initialize();
			bladeNavigationService.closeBlade(blade);
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	}

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
		$scope.isValid = formScope && formScope.$valid;
	}, true); 

	function isDirty() {
        return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
	}

	function canSave() {
        return isDirty() && formScope && formScope.$valid;
	}

//	blade.onClose = function (closeCallback) {
//		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, blade.updateTemplate, closeCallback, "platform.dialogs.notification-template-save.title", "platform.dialogs.notification-template-save.message");
//	};

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

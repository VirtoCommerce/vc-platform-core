angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.editTemplateController', ['$rootScope', '$scope', '$timeout', '$localStorage', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'FileUploader', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 
 function ($rootScope, $scope, $timeout, $localStorage, notifications, FileUploader, bladeNavigationService, dialogService) {
    var blade = $scope.blade;    
    $scope.isValid = false;

    var formScope; 
    $scope.setForm = function (form) { 
        formScope = form; 
    }

    var codemirrorEditor;
    blade.dynamicProperties = '';

    function saveTemplate() {
        var date = new Date();
        var now = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
        if (!blade.isNew) {
            blade.currentEntity.modifiedDate = now;
            blade.origEntity = angular.copy(blade.currentEntity);
        }
        else {
            blade.currentEntity.createdDate = now;
            blade.origEntity = angular.copy(blade.currentEntity);
        }
        var ind = blade.notification.templates.findIndex(function (element) {
            return (blade.currentEntity.id && element.id === blade.currentEntity.id) 
                || element.languageCode === blade.currentEntity.languageCode;
        });
        if (ind >= 0) {
            blade.notification.templates[ind] = blade.currentEntity;
        }
        else {
            blade.notification.templates.push(blade.currentEntity);
        }
    }

    $scope.saveChanges = function () {
        saveTemplate();
        blade.parentBlade.initialize();
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
     
    function setTemplate() {
        if (!blade.currentEntity) {
            blade.currentEntity = { kind: blade.notification.kind }
        }

		blade.isLoading = false;
        if (blade.currentEntity && blade.currentEntity.languageCode === undefined) { 
            blade.currentEntity.languageCode = null; 
        }
        
		$timeout(function () {
			if (codemirrorEditor) {
				codemirrorEditor.refresh();
				codemirrorEditor.focus();
			}
			blade.origEntity = angular.copy(blade.currentEntity);
		}, 1);
        
        $scope.isValid = false;
	};

	blade.initialize = function () {
		blade.isLoading = true;
        var found = _.find(blade.notification.templates, function(templ){ return !templ.isReadonly && templ.languageCode === blade.languageCode });
        if (found){
            blade.currentEntity = angular.copy(found);        
            blade.origEntity = angular.copy(blade.currentEntity);
            blade.orightml = blade.currentEntity.body;
        }

        setTemplate();
	};

     blade.renderTemplate = function () {
		var newBlade = {
			id: 'renderTemplate',
			title: 'notifications.blades.notifications-template-render.title',
			subtitle: 'notifications.blades.notifications-template-render.subtitle',
			subtitleValues: { type: blade.notificationType },
			notification: blade.notification,
			tenantId: blade.tenantId,
			tenantType: blade.tenantType,
            currentEntity: blade.currentEntity,
            languageCode: blade.currentEntity.languageCode,
			controller: 'virtoCommerce.notificationsModule.templateRenderController',
			template: 'Modules/$(VirtoCommerce.Notifications)/Scripts/blades/notifications-template-render.tpl.html'
		};

		bladeNavigationService.showBlade(newBlade, blade);
	}
     
    $scope.blade.toolbarCommands = [
        {
            name: "platform.commands.preview", icon: 'fa fa-eye',
            executeMethod: function () {
                blade.renderTemplate();
            },
            canExecuteMethod: canRender,
            permission: 'notifications:templates:read'
        }
    ];
	 
     
    function isDirty() {
        return (!angular.equals(blade.origEntity, blade.currentEntity) || blade.isNew) && blade.hasUpdatePermission();
    }
    
    function canRender() {
        return !isDirty();
	}
 
     
    $scope.$watch("blade.currentEntity", function () {
		$scope.isValid = isDirty() && formScope && formScope.$valid;
	}, true); 

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

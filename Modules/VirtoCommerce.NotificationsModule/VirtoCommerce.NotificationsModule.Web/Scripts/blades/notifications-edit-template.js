angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.editTemplateController', ['$rootScope', '$scope', '$timeout', '$localStorage', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'FileUploader', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 
 function ($rootScope, $scope, $timeout, $localStorage, notifications, FileUploader, bladeNavigationService, dialogService) {
    var blade = $scope.blade;    
    blade.updatePermission = 'platform:notification:update';
    $scope.isValid = false;
     
    var formScope; 
    $scope.setForm = function (form) { 
        formScope = form; 
    }

    var codemirrorEditor;
    var keyTemplateLocalStorage;
    blade.dynamicProperties = '';//{"lastname": "Kostyrin"}

    $scope.saveChanges = function () {
        var date = new Date();
        var now = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
        if (!blade.currentEntity.languageCode) { blade.currentEntity.languageCode = 'default'; }
        if (!blade.isNew) {
            blade.currentEntity.modified = now;
            blade.origEntity = angular.copy(blade.currentEntity);
        }
        else {
            blade.currentEntity.created = now;
            blade.origEntity = angular.copy(blade.currentEntity);
        }
        var ind = blade.notification.templates.findIndex(function (element) {
            return element.languageCode === blade.currentEntity.languageCode;
        });
        if (ind >= 0) {
            blade.notification.templates[ind] = blade.currentEntity;
        }
        else {
            blade.notification.templates.push(blade.currentEntity);
        }
        if (blade.dynamicProperties) {
            $localStorage[keyTemplateLocalStorage] = blade.dynamicProperties;    
        }
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
     
    function setTemplate(data) {
        
		blade.isLoading = false;
        if (!blade.isNew) {
            keyTemplateLocalStorage = blade.objectTypeId + '.' + blade.notification.type + '.' + blade.currentEntity.languageCode;
            var itemFromLocalStorage = $localStorage[keyTemplateLocalStorage];
            if (itemFromLocalStorage) {
                blade.dynamicProperties = itemFromLocalStorage;
            }    
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
        if (blade.languageCode) {
            var found = _.findWhere(blade.notification.templates, { languageCode: blade.languageCode });
            if (found){
                blade.currentEntity = angular.copy(found);        
                blade.origEntity = angular.copy(blade.currentEntity);
            }
            
        }
        setTemplate(blade.currentEntity);
	};

//	$scope.editorOptions = {
//		lineWrapping: true,
//		lineNumbers: true,
//		parserfile: "liquid.js",
//		extraKeys: { "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); } },
//		foldGutter: true,
//		gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
//		onLoad: function (_editor) {
//			codemirrorEditor = _editor;
//		},
//		mode: "liquid-html"
//	};
     
    $scope.$watch("blade.currentEntity", function () {
		$scope.isValid = formScope && formScope.$valid;
	}, true); 

	blade.headIcon = 'fa-envelope';

	blade.initialize();
}]);

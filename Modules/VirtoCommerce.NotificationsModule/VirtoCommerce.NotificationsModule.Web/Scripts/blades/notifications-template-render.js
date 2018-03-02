angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.templateRenderController', ['$rootScope', '$scope', '$localStorage', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.notificationsModule.notificationsModuleApi', function ($rootScope, $scope, $localStorage, bladeNavigationService, dialogService, notifications) {
	var blade = $scope.blade;
    var keyTemplateLocalStorage;
    blade.dynamicProperties = '';
    blade.originHtml = '';

	$scope.setForm = function (form) {
		$scope.formScope = form;
	}

	blade.initialize = function () {
		blade.isLoading = true;
		blade.isRender = false;
        
        keyTemplateLocalStorage = blade.tenantType + '.' + blade.notification.type + '.' + blade.currentEntity.languageCode;
        var itemFromLocalStorage = $localStorage[keyTemplateLocalStorage];
        if (itemFromLocalStorage) {
            blade.notification.context = itemFromLocalStorage;
        } 
		blade.isLoading = false;
	};
    
    blade.render = function() {
        console.log(blade.notification);
        notifications.renderTemplate({},{ text: blade.currentEntity.body, data: blade.notification.previewData }, function (data) {
            console.log(data);
            blade.originHtml = data;
        });
    }

	blade.headIcon = 'fa-play';

	blade.initialize();
}]);
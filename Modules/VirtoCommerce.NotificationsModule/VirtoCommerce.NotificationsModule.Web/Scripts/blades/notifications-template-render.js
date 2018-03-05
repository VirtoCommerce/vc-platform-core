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
        
        notifications.renderTemplate({},{ text: blade.currentEntity.body, data: blade.notification }, function (data) {
            blade.originHtml = data.html;
        });
		blade.isLoading = false;
	};

	blade.headIcon = 'fa-eye';

	blade.initialize();
}]);
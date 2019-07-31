angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.templateRenderController', ['$rootScope', '$scope', '$localStorage', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.notificationsModule.notificationsModuleApi', function ($rootScope, $scope, $localStorage, bladeNavigationService, dialogService, notifications) {
	var blade = $scope.blade;
    var keyTemplateLocalStorage;
    blade.dynamicProperties = '';
    blade.originHtml = '';

	$scope.setForm = function (form) {
		$scope.formScope = form;
	}

	function pluckAddress(address) {
		if (address) {
			return _(address).pluck('value');	
		}
		return address;
	}

	blade.initialize = function () {
		blade.isLoading = true;
		blade.isRender = false;
        var data = angular.copy(blade.notification);
        data.cc = pluckAddress(data.cc);
        data.bcc = pluckAddress(data.bcc);
        keyTemplateLocalStorage = blade.tenantType + '.' + blade.notification.type + '.' + blade.languageCode;
        var itemFromLocalStorage = $localStorage[keyTemplateLocalStorage];
        if (itemFromLocalStorage) {
            blade.notification.context = itemFromLocalStorage;
        } 
        
        notifications.renderTemplate({type: blade.notification.type, language: blade.languageCode},{ text: blade.currentEntity.body, data }, function (data) {
            blade.originHtml = data.html;
        });
		blade.isLoading = false;
	};

	blade.headIcon = 'fa-eye';

	blade.initialize();
}]);
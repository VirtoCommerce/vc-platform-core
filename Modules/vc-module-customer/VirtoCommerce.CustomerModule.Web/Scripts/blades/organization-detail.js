angular.module('virtoCommerce.customerModule')
    .controller('virtoCommerce.customerModule.organizationDetailController', ['$scope', 'platformWebApp.settings', 'platformWebApp.bladeNavigationService', function ($scope, settings, bladeNavigationService) {
    var blade = $scope.blade;

    if (blade.isNew) {
        blade.title = 'customer.blades.organization-detail.title-new';
        blade.currentEntity.parentId = blade.parentBlade.currentEntity.id;

        blade.fillDynamicProperties();
    } else {
        blade.subtitle = 'customer.blades.organization-detail.subtitle';
    }
    $scope.groups = settings.getValues({ id: 'Customer.MemberGroups' });
    $scope.openGroupsDictionarySettingManagement = function () {
        var newBlade = {
            id: 'settingDetailChild',
            isApiSave: true,
            currentEntityId: 'Customer.MemberGroups',
            parentRefresh: function (data) { $scope.groups = data; },
            controller: 'platformWebApp.settingDictionaryController',
            template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

}]);
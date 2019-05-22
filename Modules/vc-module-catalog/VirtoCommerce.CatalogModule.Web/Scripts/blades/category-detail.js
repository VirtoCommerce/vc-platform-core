angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.categoryDetailController', ['$rootScope', '$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'virtoCommerce.catalogModule.categories', 'virtoCommerce.catalogModule.catalogs', 'platformWebApp.metaFormsService', function ($rootScope, $scope, bladeNavigationService, settings, categories, catalogs, metaFormsService) {
    var blade = $scope.blade;
    blade.updatePermission = 'catalog:update';

    blade.metaFields = metaFormsService.getMetaFields("categoryDetail");

    blade.refresh = function (parentRefresh) {
        return categories.get({ id: blade.currentEntityId }, function (data) {

            if (!blade.catalog) {
                blade.catalog = catalogs.get({ id: data.catalogId });
            }

            initializeBlade(data);

            if (blade.childrenBlades) {
                _.each(blade.childrenBlades, function (x) {
                    if (x.refresh) {
                        x.refresh(blade.currentEntity);
                    }
                });
            }

            if (parentRefresh) {
                blade.parentBlade.refresh();
            }
        },
        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    };

    function initializeBlade(data) {
        blade.currentEntity = angular.copy(data);
        blade.origEntity = data;
        blade.title = data.name;
        blade.isLoading = false;
        blade.securityScopes = data.securityScopes;
    };

    blade.codeValidator = function (value) {
        var pattern = /[$+;=%{}[\]|@~!^*&()?'<>,]/;
        return !pattern.test(value);
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    };

    function canSave() {
        return isDirty() && blade.formScope && blade.formScope.$valid;
    }

    function saveChanges() {
        blade.isLoading = true;
        categories.update({}, blade.currentEntity, function (data, headers) {
            blade.refresh(true);
        },
        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    };

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback, "catalog.dialogs.category-save.title", "catalog.dialogs.category-save.message");
    };

    blade.formScope = null;
    $scope.setForm = function (form) { blade.formScope = form; }

    blade.toolbarCommands = [
		{
		    name: "platform.commands.save", icon: 'fa fa-save',
		    executeMethod: saveChanges,
		    canExecuteMethod: canSave,
		    permission: blade.updatePermission
		},
        {
            name: "platform.commands.reset", icon: 'fa fa-undo',
            executeMethod: function () {
                angular.copy(blade.origEntity, blade.currentEntity);
            },
            canExecuteMethod: isDirty,
            permission: blade.updatePermission
        }
    ];

    blade.openDictionarySettingManagement = function () {
        var newBlade = {
            id: 'settingDetailChild',
            isApiSave: true,
            currentEntityId: 'VirtoCommerce.Core.General.TaxTypes',
            parentRefresh: function (data) { blade.taxTypes = data; },
            controller: 'platformWebApp.settingDictionaryController',
            template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    blade.refresh();
    blade.taxTypes = settings.getValues({ id: 'VirtoCommerce.Core.General.TaxTypes' });
}]);

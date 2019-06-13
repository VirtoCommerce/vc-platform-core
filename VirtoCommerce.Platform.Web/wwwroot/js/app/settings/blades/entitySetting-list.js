angular.module('platformWebApp').controller('platformWebApp.entitySettingListController', ['$scope', 'platformWebApp.settings.helper', 'platformWebApp.bladeNavigationService', function ($scope, settingsHelper, bladeNavigationService) {
    var blade = $scope.blade;
    blade.title = 'platform.blades.entitySetting-list.title';

    function initializeBlade(settings) {
        blade.data = settings;
        settings = angular.copy(settings);
        fixSettingValue(settings);
        addGroupNameToShow(settings);
        convertAllowedValues(settings);

        settings = _.groupBy(settings, 'groupName');
        blade.groupNames = _.keys(settings);
        blade.currentEntities = angular.copy(settings);
        blade.origEntity = settings;
        blade.isLoading = false;
    }

    /** transform settings to editable format */
    function transformSettings(settings) {
        _.each(settings, function (setting) {

            // if setting has no value, use default
            if ((setting.value === undefined) && setting.defaultValue) {
                setting.value = setting.defaultValue;
            }

            // add groupName to show
            if (setting.groupName) {
                var paths = setting.groupName.split('|');
                setting.groupName = paths.pop();
            }

            // setting.values used to 
            setting.values = setting.isDictionary ? [{ value: { id: setting.value, name: setting.value } }] : [{ id: setting.value, value: setting.value }];

            // convert allovedValues to editable format
            if (setting.allowedValues) {
                setting.allowedValues = _.map(setting.allowedValues, function (x) {
                    return { value: x };
                });
            }

        });
    }

    $scope.editDictionary = function (node) {
        var newBlade = {
            id: "settingDetailChild",
            currentEntityId: node.name,
            controller: 'platformWebApp.settingDictionaryController',
            template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    function isDirty() {
        return !angular.equals(blade.currentEntities, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && formScope && formScope.$valid;
    }

    var formScope;
    $scope.setForm = function (form) { formScope = form; }

    $scope.cancelChanges = function () {
        angular.copy(blade.origEntity, blade.currentEntities);
        $scope.bladeClose();
    }

    $scope.saveChanges = function () {
        debugger;
        if (!blade.hasUpdatePermission()) return;

        blade.isLoading = true;
        var objects = _.flatten(_.map(blade.currentEntities, _.values));
        objects = _.map(objects, function (x) {
            x.value = x.isDictionary ? x.values[0].value.id : x.values[0].value;
            x.values = undefined;
            x.allowedValues = _.pluck(x.allowedValues, 'id');
            return x;
        });

        //settingsHelper.toApiFormat(objects);

        angular.copy(objects, blade.data);
        angular.copy(blade.currentEntities, blade.origEntity);
        $scope.bladeClose();
    };

    blade.headIcon = 'fa-wrench';
    blade.isSavingToParentObject = true;
    blade.toolbarCommands = [
        {
            name: "platform.commands.reset", icon: 'fa fa-undo',
            executeMethod: function () {
                blade.currentEntities = angular.copy(blade.origEntity);
            },
            canExecuteMethod: isDirty,
            permission: blade.updatePermission
        }
    ];

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "platform.dialogs.settings-save.title", "platform.dialogs.settings-save.message");
    };

    $scope.getDictionaryValues = function (setting, callback) {
        callback(setting.allowedValues);
    };

    // actions on load
    $scope.$watch('blade.parentBlade.currentEntity.settings', initializeBlade);
}]);

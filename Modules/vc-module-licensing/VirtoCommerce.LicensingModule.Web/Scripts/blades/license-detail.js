angular.module('virtoCommerce.licensingModule')
.controller('virtoCommerce.licensingModule.licenseDetailController', ['$scope', 'virtoCommerce.licensingModule.licenseApi', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings',
function ($scope, licenseApi, dialogService, bladeNavigationService, settings) {
    var blade = $scope.blade;
    blade.updatePermission = 'licensing:update';

    blade.refresh = function () {
        blade.isLoading = true;
        licenseApi.get({ id: blade.data.id }, blade.initialize);
    };

    blade.initialize = function (data) {
        blade.origEntity = data;
        blade.currentEntity = angular.copy(data);
        blade.isLoading = false;
    };

    $scope.cancelChanges = function () {
        blade.currentEntity = blade.origEntity;
        $scope.bladeClose();
    };
    $scope.saveChanges = function () {
        blade.isLoading = true;

        if (blade.isNew) {
            licenseApi.save(blade.currentEntity, function (result) {
                angular.copy(blade.currentEntity, blade.origEntity);
                if (blade.saveCallback)
                    blade.saveCallback(result);
            });
        } else {
            licenseApi.update(blade.currentEntity, function () {
                blade.refresh();
                blade.parentBlade.refresh();
            });
        }
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope.$valid;
    }

    $scope.setForm = function (form) { $scope.formScope = form; };

    if (!blade.isNew) {
        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: $scope.saveChanges,
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
            },
            {
                name: "licensing.commands.issue", icon: 'fa fa-print',
                executeMethod: function () {
                    window.open('api/licenses/download/' + blade.currentEntity.activationCode, '_blank');
                },
                canExecuteMethod: function () { return !isDirty(); },
                permission: "licensing:issue"
            }
        ];
    }

    /* Datepicker */
    $scope.datepickers = {};
    $scope.today = new Date();

    $scope.open = function ($event, which) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.datepickers[which] = true;
    };

    settings.getValues({ id: 'Licensing.LicenseType' }, function (data) {
        $scope.types = data;
        if (blade.currentEntity && !blade.currentEntity.type) {
            blade.currentEntity.type = $scope.types[0];
        }
    });

    $scope.openDictionarySettingManagement = function () {
        var newBlade = new DictionarySettingDetailBlade('Licensing.LicenseType');
        newBlade.parentRefresh = function (data) {
            $scope.types = data;
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };


    if (blade.isNew) {
        angular.extend(blade, {
            title: 'licensing.blades.license-detail.title-new'
        });
        blade.initialize(blade.data);
    } else {
        angular.extend(blade, {
            title: 'licensing.blades.license-detail.title'
        });
        blade.refresh();
    }

}]);
angular.module('virtoCommerce.taxModule')
.controller('virtoCommerce.taxModule.taxProviderDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'virtoCommerce.taxModule.taxProviders', function ($scope, bladeNavigationService, dialogService, settings, taxProviders) {
    var blade = $scope.blade;

    function initializeBlade(data) {    
      
        blade.currentEntity = angular.copy(data);
        blade.origEntity = data;
        blade.isLoading = false;
    }

    blade.refresh = function(parentRefresh) {
        blade.isLoading = true;
        if (blade.taxProvider.id) {
            taxProviders.get({ id: blade.taxProvider.id },
                function(data) {
                    initializeBlade(data);
                    if (parentRefresh) {
                        blade.parentBlade.refresh();
                    }
                },
                function(error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        } else {
            initializeBlade(blade.taxProvider);
        }
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty();
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "stores.dialogs.tax-provider-save.title", "stores.dialogs.tax-provider-save.message");
    };

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    };

    $scope.saveChanges = function () {
        blade.isLoading = true;
       
        blade.currentEntity.storeId = blade.storeId;
        taxProviders.update({}, blade.currentEntity, function (data) {
            blade.taxProvider.id = data.id;
            blade.refresh(true);
        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });

    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    };

    blade.headIcon = 'fa-archive';

    blade.toolbarCommands = [
        {
            name: "platform.commands.save",
            icon: 'fa fa-save',
            executeMethod: $scope.saveChanges,
            canExecuteMethod: canSave,
            permission: blade.updatePermission
        },
        {
            name: "platform.commands.reset",
            icon: 'fa fa-undo',
            executeMethod: function () {
                angular.copy(blade.origEntity, blade.currentEntity);
            },
            canExecuteMethod: isDirty,
            permission: blade.updatePermission
        }      
    ];

    blade.refresh();
}]);

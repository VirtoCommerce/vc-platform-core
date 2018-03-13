angular.module('virtoCommerce.coreModule.common')
    .controller('virtoCommerce.coreModule.common.coreAddressDetailController', ['$scope', 'platformWebApp.common.countries', 'platformWebApp.dialogService', 'platformWebApp.metaFormsService', 'platformWebApp.bladeNavigationService', function ($scope, countries, dialogService, metaFormsService, bladeNavigationService) {
    var blade = $scope.blade;

    blade.addressTypes = ['Billing', 'Shipping', 'BillingAndShipping'];
    blade.metaFields = metaFormsService.getMetaFields('addressDetails');
    blade.origEntity = blade.currentEntity;
    blade.currentEntity = angular.copy(blade.origEntity);
    blade.countries = countries.query();

    if (blade.currentEntity.isNew) {
        blade.currentEntity.addressType = blade.addressTypes[1];
    }

    blade.toolbarCommands = [{
        name: "platform.commands.reset", icon: 'fa fa-undo',
        executeMethod: function () {
            angular.copy(blade.origEntity, blade.currentEntity);
        },
        canExecuteMethod: isDirty
    }, {
        name: "platform.commands.delete", icon: 'fa fa-trash-o',
        executeMethod: deleteEntry,
        canExecuteMethod: function () {
            return !blade.currentEntity.isNew;
        }
    }];

    blade.isLoading = false;

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "core.dialogs.address-save.title", "core.dialogs.address-save.message");
    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    };

    $scope.isValid = function () {
        return $scope.formScope && $scope.formScope.$valid;
    }

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    }

    $scope.saveChanges = function () {
        if (blade.confirmChangesFn) {
            blade.confirmChangesFn(blade.currentEntity);
        };
        angular.copy(blade.currentEntity, blade.origEntity);
        $scope.bladeClose();
    };

    $scope.$watch('blade.currentEntity.countryCode', function (countryCode) {
        var country;
        if (countryCode && (country = _.findWhere(blade.countries, { id: countryCode }))) {
            blade.currentEntity.countryName = country.name;
        }
    });

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity);
    }

    function canSave() {
        return isDirty();
    }

    function deleteEntry() {
        var dialog = {
            id: "confirmDelete",
            title: "core.dialogs.address-delete.title",
            message: "core.dialogs.address-delete.message",
            callback: function (remove) {
                if (remove) {
                    if (blade.deleteFn) {
                        blade.deleteFn(blade.currentEntity);
                    };
                    $scope.bladeClose();
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }
}]);

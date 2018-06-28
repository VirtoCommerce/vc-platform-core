angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.inventoryDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.inventoryModule.inventories', 'platformWebApp.metaFormsService', function ($scope, bladeNavigationService, inventories, metaFormsService) {
    var blade = $scope.blade;
    blade.updatePermission = 'inventory:update';

    blade.refresh = function() {
        blade.isLoading = true;
        blade.parentBlade.refresh().then(function(results) {
            var data = _.findWhere(results, { fulfillmentCenterId: blade.data.fulfillmentCenterId });

            initializeBlade(data);
        });
    };

    blade.metaFields = blade.metaFields ? blade.metaFields : metaFormsService.getMetaFields('inventoryDetails');

    function initializeBlade(data) {
        blade.currentEntity = angular.copy(data);
        blade.origEntity = data;
        blade.isLoading = false;
    }

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope && $scope.formScope.$valid;
    }

    $scope.setForm = function (form) { $scope.formScope = form; };

    $scope.saveChanges = function () {
        blade.isLoading = true;
        inventories.update({ id: blade.itemId }, blade.currentEntity, function () {
            blade.refresh();
        });
    };

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "inventory.dialogs.inventory-save.title", "inventory.dialogs.inventory-save.message");
    };

    blade.headIcon = 'fa-cubes';
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
        }
    ];

    // datepicker
    $scope.datepickers = {
        pod: false,
        bod: false
    };

    $scope.today = new Date();

    $scope.open = function ($event, which) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.datepickers[which] = true;
    };

    // on load
    initializeBlade(blade.data);
}]);
